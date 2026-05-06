using Xunit;

public sealed class Version020CoreTests
{
	[Fact]
	public void VersionInfo_Reports020()
	{
		Assert.Equal("0.3.0", VersionInfo.Version);
		Assert.Equal("5", VersionInfo.SaveVersion);
	}

	[Fact]
	public void QuickSlots_AssignAndCycle()
	{
		InventoryQuickSlotState quick = new(3);
		Assert.True(quick.Assign(1, 5));
		Assert.True(quick.TryGetInventorySlot(1, out int slot));
		Assert.Equal(5, slot);
		Assert.Equal(1, quick.Cycle(1));
		Assert.Equal(2, quick.Cycle(1));
		Assert.Equal(1, quick.Cycle(-1));
	}

	[Fact]
	public void Consumable_CanRestoreFoodAndWater()
	{
		InventoryState inventory = new(4, 50);
		ItemData food = new() { Id = "canned_food", DisplayName = "Food", IsConsumable = true, FoodRestore = 40, MaxStackSize = 5, Weight = 1 };
		inventory.TryAddItem(new InventoryItemDefinition(food.Id, food.MaxStackSize, food.Weight), 1);
		SurvivalNeedsState needs = new();
		needs.Consume(80, 0);

		Assert.True(InventoryUseLogic.TryUseConsumableFromSlot(inventory, 0, food, new PlayerStatsState(100), needs));
		Assert.Equal(60, needs.Food);
		Assert.True(inventory.Slots[0].IsEmpty);
	}

	[Fact]
	public void SurvivalNeeds_TickConsumesNeedsAndCanDamageCriticalPlayer()
	{
		SurvivalNeedsState needs = new(10, 10);
		PlayerStatsState stats = new(20);
		needs.Consume(10, 10);

		SurvivalNeedsUpdateResult result = SurvivalNeedsLogic.Tick(needs, stats, 8f, false);

		Assert.True(result.IsCritical);
		Assert.True(stats.CurrentHealth < stats.MaxHealth);
	}

	[Fact]
	public void StorageWithdrawalPlanner_ReturnsMissingAmounts()
	{
		StorageState storage = new();
		storage.AddItem("metal", 2, 99);

		var plan = StorageWithdrawalPlanner.Plan(storage, new[] { new CraftIngredient("metal", 5) });

		Assert.Single(plan);
		Assert.Equal(2, plan[0].PlannedAmount);
		Assert.Equal(3, plan[0].MissingAmount);
	}

	[Fact]
	public void CraftingBatchPlanner_ReportsMaxBatchesAndMissingForDesiredAmount()
	{
		StorageState storage = new();
		storage.AddItem("wood", 5, 99);
		storage.AddItem("metal", 1, 99);
		CraftRecipe recipe = new("r", "R", new[] { new CraftIngredient("wood", 2), new CraftIngredient("metal", 1) }, "out", 1);

		CraftingBatchPlan plan = CraftingBatchPlanner.BuildPlan(storage, recipe, 2);

		Assert.Equal(1, plan.MaxBatches);
		Assert.Single(plan.MissingIngredients);
		Assert.Equal("metal", plan.MissingIngredients[0].ItemId);
	}

	[Fact]
	public void ResourceRespawnPlanner_IncreasesRespawnsWithBaseLevel()
	{
		ResourceRespawnPlan plan = ResourceRespawnPlanner.BuildPlan(6, 3, 4);
		Assert.True(plan.NodesToRespawn >= 4);
		Assert.True(plan.ShouldEscalateDanger);
		Assert.Equal(1, plan.BonusCharges);
	}

	[Fact]
	public void EnemySpawnPlanner_ScalesWithDangerAndTime()
	{
		EnemySpawnPlan low = EnemySpawnPlanner.BuildPlan(1, 0);
		EnemySpawnPlan high = EnemySpawnPlanner.BuildPlan(4, 12);
		Assert.True(high.TotalEnemies > low.TotalEnemies);
		Assert.True(high.ArmoredEnemies > 0);
	}

	[Fact]
	public void ExpeditionScore_GradesSuccessfulRuns()
	{
		ExpeditionScore score = ExpeditionScoreCalculator.Calculate(20, 3, 10);
		Assert.True(score.TotalScore >= 80);
		Assert.True(score.Grade == "A" || score.Grade == "S");
	}

	[Fact]
	public void BaseUpgradePlanner_MarksAffordableUpgrades()
	{
		StorageState storage = new();
		storage.AddItem("wooden_plank", 10, 99);
		storage.AddItem("metal", 10, 99);
		var plan = BaseUpgradePlanner.BuildPlan(storage, BasicBaseUpgradeCatalog.CreateUpgrades());
		Assert.Contains(plan, p => p.Upgrade.Id == "storage_box" && p.CanApply);
	}

	[Fact]
	public void SaveSlotManifest_RejectsDuplicateSlot()
	{
		SaveSlotManifest manifest = new();
		Assert.True(manifest.Add(new SaveSlotDescriptor(1, "save_1.json")));
		Assert.False(manifest.Add(new SaveSlotDescriptor(1, "save_copy.json")));
		Assert.Equal(1, manifest.GetHighestSlot().SlotIndex);
	}

	[Fact]
	public void QuestObjectiveTracker_CompletesAllObjectives()
	{
		QuestObjectiveTracker020 tracker = new();
		tracker.Register("collect_wood", 3);
		tracker.Register("return_base", 1);
		Assert.False(tracker.AreAllCompleted());
		tracker.AddProgress("collect_wood", 3);
		tracker.AddProgress("return_base", 1);
		Assert.True(tracker.AreAllCompleted());
	}

	[Fact]
	public void ControlHints_IncludeBaseAndConsumableHints()
	{
		string text = ControlHintBuilder020.Build(true, true, true);
		Assert.Contains("Shift", text);
		Assert.Contains("H", text);
		Assert.Contains("База", text);
	}

	[Fact]
	public void Version020CoverageReport_TracksDeferredWorkHonestly()
	{
		Version020CoverageReport report = new(new[] { "inventory", "crafting" }, new[] { "procedural_world" });
		Assert.True(report.HasDeferredWork);
		Assert.Contains("inventory", report.CompletedBranches);
	}

	[Fact]
	public void Migration_AddsNeedsToOldSave()
	{
		SaveGameData data = new() { Version = 3, PlayerMaxHealth = 100, PlayerHealth = 80 };
		SaveGameData migrated = SaveGameMigrationService.Migrate(data);
		Assert.Equal(5, migrated.Version);
		Assert.Equal(100, migrated.PlayerMaxFood);
		Assert.Equal(100, migrated.PlayerMaxWater);
	}
}
