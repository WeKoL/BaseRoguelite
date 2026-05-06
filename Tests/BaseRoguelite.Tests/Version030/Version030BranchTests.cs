using System;
using System.Linq;
using Xunit;

public sealed class Version030BranchTests
{
	[Fact]
	public void VersionInfo_Reports030AndSave5()
	{
		Assert.Equal("0.3.0", VersionInfo.Version);
		Assert.Equal("5", VersionInfo.SaveVersion);
		Assert.Contains("14", VersionInfo.BranchSummary);
	}

	[Fact]
	public void BranchCompletionReport_CoversAllBranchesAndCanBeReleaseCandidate()
	{
		var items = Enumerable.Range(1, 14).Select(i => new BranchCompletionItem030(i, $"branch_{i}", BranchCompletionStatus030.Integrated, 3, i <= 12));
		BranchCompletionReport030 report = new(items);
		Assert.True(report.CoversAllFourteenBranches);
		Assert.True(report.HasPlayableHooksForMostBranches);
		Assert.True(report.IsReleaseCandidate);
	}

	[Fact]
	public void ReleaseGate_BlocksWhenTestsAreTooLow()
	{
		var report = new BranchCompletionReport030(Enumerable.Range(1, 14).Select(i => new BranchCompletionItem030(i, $"b{i}", BranchCompletionStatus030.Integrated, 3, true)));
		SaveGameData save = new() { PlayerMaxHealth = 100, PlayerHealth = 80, PlayerMaxFood = 100, PlayerFood = 80, PlayerMaxWater = 100, PlayerWater = 80 };
		ReleaseGateResult030 result = ReleaseGate030.Evaluate(report, SaveGameValidator.Validate(save), 10);
		Assert.False(result.CanRelease);
		Assert.Contains("not_enough_tests_for_030", result.Blockers);
	}

	[Fact]
	public void InventoryLoadoutPreset_AssignsPurposes()
	{
		InventoryLoadoutPreset030 preset = new("raid");
		Assert.True(preset.AssignPurpose(0, "weapon"));
		Assert.True(preset.AssignPurpose(2, "medicine"));
		Assert.Equal(new[] { 2 }, preset.FindSlotsByPurpose("medicine"));
	}

	[Fact]
	public void EquipmentRepair_QuotesAndAppliesRepair()
	{
		EquipmentRepairQuote030 quote = EquipmentRepairLogic030.Quote(20, 100, 30);
		Assert.Equal(3, quote.RequiredAmount);
		Assert.Equal(80, quote.DurabilityRestored);
		Assert.Equal(80, EquipmentRepairLogic030.ApplyRepair(20, 100, 2, 30));
	}

	[Fact]
	public void StorageReservationSystem_TracksAvailableAmounts()
	{
		StorageState storage = new();
		storage.AddItem("metal", 10, 99);
		StorageReservationSystem030 reservations = new();
		reservations.Reserve("metal", 4);
		Assert.Equal(6, reservations.GetAvailable(storage, "metal"));
	}

	[Fact]
	public void SmartStorageTransferPlanner_KeepsProtectedItems()
	{
		InventoryState inventory = new(4, 50);
		inventory.TryAddItem(new InventoryItemDefinition("simple_medkit", 5, 1), 1);
		inventory.TryAddItem(new InventoryItemDefinition("metal", 99, 1), 3);
		SmartStorageTransferPlan030 plan = SmartStorageTransferPlanner030.BuildPlan(inventory.Slots, new[] { "simple_medkit" });
		Assert.Contains("simple_medkit", plan.KeepInInventory);
		Assert.Contains("metal", plan.MoveToStorage);
	}

	[Fact]
	public void RecipeDiscovery_FiltersKnownRecipes()
	{
		RecipeDiscoveryState030 discovery = new();
		discovery.Learn("a");
		var recipes = new[]
		{
			new CraftRecipe("a", "A", Array.Empty<CraftIngredient>(), "out_a", 1),
			new CraftRecipe("b", "B", Array.Empty<CraftIngredient>(), "out_b", 1)
		};
		Assert.Single(discovery.FilterKnown(recipes));
	}

	[Fact]
	public void CraftQueueProcessor_CompletesQueuedRecipe()
	{
		CraftRecipe recipe = new("craft_a", "A", Array.Empty<CraftIngredient>(), "out", 2, string.Empty, 0, 3);
		CraftQueueProcessor030 processor = new();
		Assert.True(processor.Enqueue(recipe));
		Assert.Empty(processor.Tick(1, new[] { recipe }));
		var completed = processor.Tick(3, new[] { recipe });
		Assert.Single(completed);
		Assert.Equal("out", completed[0].OutputItemId);
	}

	[Fact]
	public void StatusEffectTick_AppliesBleedingDamage()
	{
		PlayerStatsState stats = new(50);
		PlayerStatusEffectCollection effects = new();
		effects.Add(new PlayerStatusEffectState(PlayerStatusEffectKind.Bleeding, 10));
		StatusEffectTickResult030 result = StatusEffectTickSystem030.Tick(effects, stats, 4f);
		Assert.True(result.HealthDamage > 0);
		Assert.True(stats.CurrentHealth < 50);
	}

	[Fact]
	public void SurvivalConditionPlanner_AsksToReturnWhenCritical()
	{
		PlayerStatsState stats = new(100);
		stats.TakeDirectDamage(90);
		SurvivalNeedsState needs = new();
		SurvivalCondition030 condition = SurvivalConditionPlanner030.Evaluate(stats, needs, 2);
		Assert.True(condition.ShouldReturnToBase);
		Assert.Equal("critical", condition.Severity);
	}

	[Fact]
	public void ToolDurability_BreaksAndRepairs()
	{
		ToolDurabilityState030 tool = new("pickaxe", 10);
		Assert.Equal(10, tool.Spend(10));
		Assert.True(tool.IsBroken);
		Assert.Equal(5, tool.Repair(5));
		Assert.False(tool.IsBroken);
	}

	[Fact]
	public void ResourceBiomePlanner_ReturnsRareBiomeForDanger()
	{
		ResourceBiome030 biome = ResourceBiomePlanner030.PickForDanger(5);
		Assert.Equal("deep_ruins", biome.Id);
		Assert.Contains("generator_part", biome.ResourceItemIds);
	}

	[Fact]
	public void RangedAttack_SpendsAmmoAndDamagesEnemy()
	{
		EnemyState enemy = new("m", 30, 0, 5);
		RangedAttackResult030 result = RangedAttackLogic030.Shoot(enemy, 3, 12, 30, 100);
		Assert.True(result.Fired);
		Assert.True(result.Hit);
		Assert.Equal(1, result.AmmoSpent);
		Assert.Equal(18, enemy.CurrentHealth);
	}

	[Fact]
	public void EnemyWaveDirector_ScalesWithDangerNoiseAndTime()
	{
		EnemyWavePlan030 weak = EnemyWaveDirector030.BuildWave(1, 0, 0);
		EnemyWavePlan030 hard = EnemyWaveDirector030.BuildWave(5, 20, 9);
		Assert.True(hard.Total > weak.Total);
		Assert.True(hard.ArmoredEnemies > 0);
	}

	[Fact]
	public void MapTilePlanner_GeneratesBaseAndFarTiles()
	{
		var tiles = MapTilePlanner030.GenerateSquareAroundBase(3);
		Assert.Contains(tiles, t => t.X == 0 && t.Y == 0 && t.ZoneKind == WorldZoneKind.Base);
		Assert.Contains(tiles, t => t.DangerLevel >= 4);
	}

	[Fact]
	public void PointOfInterestGenerator_AddsHighDangerPoints()
	{
		var points = PointOfInterestGenerator030.GenerateForDanger(5);
		Assert.Contains(points, p => p.Id == "old_lab");
	}

	[Fact]
	public void BasePowerGrid_DetectsOverload()
	{
		BasePowerGrid030 grid = new(5);
		grid.AddConsumer("workbench", 3);
		grid.AddConsumer("turret", 4);
		Assert.True(grid.IsOverloaded);
	}

	[Fact]
	public void BaseRaidResolver_LosesResourcesWhenDefenseIsLow()
	{
		BaseRaidResult030 result = BaseRaidResolver030.Resolve(3, 8, 20);
		Assert.False(result.Defended);
		Assert.True(result.ResourcesLost > 0);
	}

	[Fact]
	public void SaveDiff_DetectsStorageProgress()
	{
		SaveGameData before = new() { PlayerMaxHealth = 100, PlayerHealth = 100 };
		SaveGameData after = new() { PlayerMaxHealth = 100, PlayerHealth = 100 };
		after.StorageItems.Add(new SaveSlotItemData { ItemId = "metal", Amount = 1, MaxStackSize = 99 });
		SaveDiff030 diff = SaveDiffBuilder030.Compare(before, after);
		Assert.True(diff.HasGameplayProgress);
		Assert.Equal(1, diff.StorageDelta);
	}

	[Fact]
	public void SaveBackupPolicy_CreatesBackupForImportantProgress()
	{
		SaveBackupPolicy030 policy = new(3, 60);
		Assert.True(policy.ShouldCreateBackup(DateTime.UtcNow, DateTime.UtcNow, true));
	}

	[Fact]
	public void HudViewModelBuilder_IncludesWarningForCriticalNeeds()
	{
		SurvivalNeedsState needs = new(10, 10);
		needs.Consume(10, 0);
		HudViewModel030 hud = HudViewModelBuilder030.Build(new PlayerStatsState(100), needs, new WorldZoneState(WorldZoneKind.Danger, 4, "Свалка"));
		Assert.Contains("Критический", hud.WarningText);
		Assert.Contains("Свалка", hud.ZoneText);
	}

	[Fact]
	public void MenuNotificationCenter_DrainsMessages()
	{
		MenuNotificationCenter030 center = new();
		center.Push("one");
		center.Push("two");
		Assert.Equal(2, center.Drain().Count);
		Assert.Equal(0, center.Count);
	}

	[Fact]
	public void FeedbackTimeline_ReturnsEventsUpToTime()
	{
		FeedbackTimeline030 timeline = new();
		timeline.Add(0.2f, "hit");
		timeline.Add(2f, "loot");
		Assert.Single(timeline.GetEventsUpTo(1f));
	}

	[Fact]
	public void AudioMixPlan_ClampsVolume()
	{
		AudioMixPlan030 mix = new();
		mix.SetVolume(AudioCueKind.Hit, 5f);
		Assert.Equal(1f, mix.GetVolume(AudioCueKind.Hit));
	}

	[Fact]
	public void QuestEventRouter_ResolvesObjectivesForEvent()
	{
		QuestEventRouter030 router = new();
		router.Register("pickup_metal", "collect_metal");
		Assert.Contains("collect_metal", router.ResolveObjectives("pickup_metal"));
	}

	[Fact]
	public void JournalChapterState_CompletesRequiredSteps()
	{
		JournalChapterState030 chapter = new("tutorial");
		chapter.CompleteStep("collect");
		chapter.CompleteStep("craft");
		Assert.True(chapter.IsComplete(new[] { "collect", "craft" }));
	}

	[Fact]
	public void Version030Catalog_AddsNewCraftRecipes()
	{
		var recipes = BasicCraftRecipeCatalog.CreateRecipes();
		Assert.Contains(recipes, r => r.Id == "craft_water_filter");
		Assert.Contains(recipes, r => r.Id == "craft_camp_beacon");
	}

	[Fact]
	public void Version030Catalog_AddsNewBaseUpgrades()
	{
		var upgrades = BasicBaseUpgradeCatalog.CreateUpgrades();
		Assert.Contains(upgrades, u => u.Id == "radio_tower");
		Assert.Contains(upgrades, u => u.Id == "defensive_walls");
	}

	[Fact]
	public void SaveMigration_UpgradesOldSaveToVersion5()
	{
		SaveGameData old = new() { Version = 1, PlayerMaxHealth = 100, PlayerHealth = 70 };
		SaveGameData migrated = SaveGameMigrationService.Migrate(old);
		Assert.Equal(5, migrated.Version);
		Assert.Equal("0.3.0", migrated.BuildVersion);
		Assert.True(migrated.PlayerMaxStamina > 0);
	}
}
