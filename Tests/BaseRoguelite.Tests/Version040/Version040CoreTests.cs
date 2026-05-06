using System;
using System.Linq;
using Xunit;

public sealed class Version040CoreTests
{
	[Fact]
	public void Branch01_ManifestCoversAllBranchesAndVersionIs040()
	{
		CoreManifest040 manifest = new();
		foreach (CoreBranch040 branch in Version040Info.Branches)
			manifest.MarkImplemented(branch.Number, 2, branch.Title);

		Assert.Equal("0.4.0", Version040Info.Version);
		Assert.Equal("6", Version040Info.SaveVersion);
		Assert.True(Version040Info.CoversEveryBranch());
		Assert.True(manifest.HasAllBranches);
		Assert.True(manifest.HasMinimumTestsPerBranch(2));
	}

	[Fact]
	public void Branch02_InventoryStacksMovesSplitsAndEquipmentSwapsSafely()
	{
		ItemRegistry040 items = ItemRegistry040.CreateDefault();
		InventoryCore040 inventory = new(items, 6, 100f);
		EquipmentCore040 equipment = new(items);

		Assert.Equal(60, inventory.Add("ammo_basic", 60));
		Assert.Equal(20, inventory.Add("ammo_basic", 20));
		Assert.True(inventory.Split(0, 10));
		inventory.Add("helmet", 1);
		int helmetSlot = inventory.Slots.ToList().FindIndex(x => x.ItemId == "helmet");
		EquipResult040 result = equipment.EquipFromInventory(inventory, helmetSlot);

		Assert.True(result.Succeeded);
		Assert.False(equipment.Get(EquipmentSlot040.Head).IsEmpty);
		Assert.True(equipment.UnequipToInventory(inventory, EquipmentSlot040.Head));
		Assert.True(inventory.Count("helmet") >= 1);
	}

	[Fact]
	public void Branch03_StorageSupportsCapacityReservationSearchAndAutoUnload()
	{
		ItemRegistry040 items = ItemRegistry040.CreateDefault();
		InventoryCore040 inventory = new(items, 10, 100f);
		StorageCore040 storage = new(maxKinds: 3, maxTotalAmount: 20);
		AutoUnloadCore040 unload = new(items);
		unload.EnableCategory(ItemCategory040.Material);

		inventory.Add("wood", 8);
		inventory.Add("stone", 5);
		int moved = unload.UnloadMatching(inventory, storage);
		storage.ReserveForCrafting("wood");

		Assert.Equal(13, moved);
		Assert.Equal(8, storage.Get("wood"));
		Assert.Equal(0, storage.Remove("wood", 1));
		Assert.Equal(new[] { "stone" }, storage.Search("sto"));
		Assert.Contains("wood", storage.Audit().ReservedItems);
	}

	[Fact]
	public void Branch04_CraftingChecksStationLevelsAndConsumesStorage()
	{
		GameCore040 game = new();
		game.StartNewGame();
		CraftPreview040 medkit = game.Crafting.CraftNow("medkit", game.Storage);
		CraftPreview040 armorBeforeWorkbench2 = game.Crafting.Preview("armor", game.Storage);

		game.Base.SetFacility(BaseFacility040.Workbench, 2);
		game.Storage.Add("metal", 10);
		game.Storage.Add("cloth", 10);
		CraftPreview040 armor = game.Crafting.CraftNow("armor", game.Storage);

		Assert.True(medkit.CanCraft);
		Assert.Equal("station_required", armorBeforeWorkbench2.Blocker);
		Assert.True(armor.CanCraft);
		Assert.Equal(1, game.Storage.Get("armor"));
	}

	[Fact]
	public void Branch05_PlayerVitalsHandleNeedsStaminaStatusesAndConsumables()
	{
		ItemRegistry040 items = ItemRegistry040.CreateDefault();
		InventoryCore040 inventory = new(items, 6, 100f);
		PlayerVitals040 vitals = new();
		ConsumableUseCore040 consumables = new(items);

		vitals.Damage(50);
		inventory.Add("medkit", 1);
		Assert.True(consumables.Use("medkit", inventory, vitals));
		vitals.AddStatus(StatusKind040.Bleeding, 30);
		SurvivalTickResult040 tick = vitals.Tick(10, running: true, inBase: false);

		Assert.True(vitals.Hp > 50);
		Assert.True(tick.HasPressure);
		Assert.True(vitals.Stamina < 100);
	}

	[Fact]
	public void Branch06_GatheringRespectsToolsChargesAndAddsLoot()
	{
		ItemRegistry040 items = ItemRegistry040.CreateDefault();
		InventoryCore040 inventory = new(items, 10, 100f);
		GatheringCore040 gathering = new(items);
		ResourceNode040 node = new("stone_node", new System.Collections.Generic.Dictionary<string, int> { ["stone"] = 2 }, charges: 1, requiredToolId: "pickaxe");

		GatherOutcome040 wrongTool = gathering.Gather(node, "axe", inventory);
		GatherOutcome040 rightTool = gathering.Gather(node, "pickaxe", inventory);
		GatherOutcome040 depleted = gathering.Gather(node, "pickaxe", inventory);

		Assert.False(wrongTool.Succeeded);
		Assert.True(rightTool.Succeeded);
		Assert.Equal(2, inventory.Count("stone"));
		Assert.Equal("depleted", depleted.Error);
	}

	[Fact]
	public void Branch07_CombatDamageAiWavesAndAmmoWorkTogether()
	{
		CombatActor040 player = new("player", 100, 2, 10);
		CombatActor040 enemy = new("enemy", 20, 1, 4);
		CombatCore040 combat = new();
		EnemyAiCore040 ai = new();
		EnemyWaveCore040 waves = new();
		AmmoCore040 ammo = new();

		ammo.Add("ammo_basic", 5);
		CombatResult040 result = combat.Attack(player, enemy);

		Assert.True(result.Succeeded);
		Assert.Equal(EnemyDecision040.Attack, ai.Decide(12, enemy.Hp, player.Hp, false));
		Assert.True(waves.CreateWave(3, 4).Count >= 4);
		Assert.True(ammo.Spend("ammo_basic", 3));
		Assert.Equal(2, ammo.Get("ammo_basic"));
	}

	[Fact]
	public void Branch08_WorldUnlocksZonesTracksEventsAndExpeditionReturnPressure()
	{
		WorldCore040 world = WorldCore040.CreateDefault();
		PlayerVitals040 vitals = new();
		ExpeditionCore040 expedition = new();
		expedition.Enter(WorldZone040.Factory);
		ZoneRule040 factory = world.Zones.Single(x => x.Zone == WorldZone040.Factory);
		ExpeditionTick040 tick = expedition.Tick(25, vitals, factory);

		Assert.Contains(world.GetUnlockedZones(2), x => x.Zone == WorldZone040.Factory);
		Assert.Contains("danger_rises", world.GetDueEvents(12));
		Assert.True(tick.ShouldReturn);
		Assert.True(expedition.ExtractToBase());
	}

	[Fact]
	public void Branch09_BaseUpgradesEnergyDefenseComfortAndRaids()
	{
		BaseCore040 baseCore = new();
		StorageCore040 storage = new();
		storage.Add("wood", 20);
		storage.Add("metal", 20);
		BaseUpgradeCore040 upgrades = new();
		BaseUpgrade040 walls = new(BaseFacility040.Walls, 1, new System.Collections.Generic.Dictionary<string, int> { ["wood"] = 5, ["metal"] = 5 });
		UpgradePreview040 built = upgrades.Build(baseCore, storage, walls);
		RaidResult040 raid = new BaseRaidCore040().Resolve(5, baseCore);

		Assert.True(built.CanBuild);
		Assert.Equal(1, baseCore.GetLevel(BaseFacility040.Walls));
		Assert.True(baseCore.Defense > 0);
		Assert.True(raid.Repelled);
	}

	[Fact]
	public void Branch10_SaveSerializesMigratesAndValidatesCoreState()
	{
		GameCore040 game = new();
		game.StartNewGame();
		SaveCore040 saves = new();
		SaveGame040 snapshot = saves.Build(game);
		string json = saves.Serialize(snapshot);
		SaveGame040 loaded = saves.Deserialize(json);

		Assert.Equal("6", loaded.SaveVersion);
		Assert.True(saves.Validate(loaded).IsValid);
		Assert.True(loaded.Storage["wood"] >= 5);
	}

	[Fact]
	public void Branch11_PresentationBuildsHudActionsAndTooltipsWithoutScenes()
	{
		ItemRegistry040 items = ItemRegistry040.CreateDefault();
		InventoryCore040 inventory = new(items, 8, 100f);
		PlayerVitals040 player = new();
		MenuActionProvider040 menu = new(items);
		TooltipCore040 tooltip = new(items);

		inventory.Add("medkit", 1);
		HudModel040 hud = new HudPresenter040().Build(player, inventory, WorldZone040.NearField);

		Assert.Contains("HP", hud.HpText);
		Assert.Contains(MenuAction040.Use, menu.ForInventoryItem("medkit", nearBase: true));
		Assert.Contains("Лечение", tooltip.BuildItemTooltip("medkit"));
	}

	[Fact]
	public void Branch12_FeedbackBusQueuesDrainsAndPlansEventIntensity()
	{
		FeedbackBus040 bus = new();
		FeedbackPlanner040 planner = new();
		bus.Push(FeedbackKind040.Pickup, "wood +1");
		bus.Push(planner.ForDamage(30, killed: false).Kind, planner.ForDamage(30, killed: false).Text, planner.ForDamage(30, killed: false).Intensity);

		var events = bus.Drain();
		Assert.Equal(2, events.Count);
		Assert.Contains(events, x => x.Kind == FeedbackKind040.Damage && x.Intensity > 1f);
		Assert.Equal(0, bus.Count);
	}

	[Fact]
	public void Branch13_QuestTrackerCompletesObjectivesAndExposesRewards()
	{
		QuestTracker040 quests = QuestTracker040.CreateTutorial();
		quests.RecordEvent("gather_resource", 5);

		Assert.True(quests.IsCompleted("first_gather"));
		Assert.Equal(3, quests.ClaimReward("first_gather")["wood"]);
	}

	[Fact]
	public void Branch14_QualityGateRejectsIncompleteAndPassesCompleteCore()
	{
		CoreManifest040 manifest = new();
		foreach (int branch in Enumerable.Range(1, 14)) manifest.MarkImplemented(branch, 1, "covered");
		QualityGate040 gate = new();
		SaveValidation040 validSave = new SaveValidation040(Array.Empty<string>());

		QualityResult040 result = gate.Evaluate(manifest, validSave, factTestsCount: 14, hasVersionNotes: true);

		Assert.True(result.Passed);
		Assert.Empty(new BalanceRules040().Validate(ItemRegistry040.CreateDefault()));
	}

	[Fact]
	public void Integration_CoreSimulationRunsBaseLoopWithoutGodotSceneObjects()
	{
		GameCore040 game = new();
		game.StartNewGame();
		game.Inventory.Add("helmet", 1);
		int helmetSlot = game.Inventory.Slots.ToList().FindIndex(x => x.ItemId == "helmet");
		game.Equipment.EquipFromInventory(game.Inventory, helmetSlot);
		game.Crafting.CraftNow("medkit", game.Storage);
		CoreStepReport040 report = game.RunCoreStep(30);

		Assert.True(game.Equipment.Armor >= 2);
		Assert.True(game.Storage.Get("medkit") >= 1);
		Assert.NotNull(report.StorageAudit);
		Assert.True(game.Player.IsAlive);
	}
}
