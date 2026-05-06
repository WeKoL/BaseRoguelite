using System;
using System.Linq;
using Xunit;

public sealed class Version031BranchExpansionTests
{
	[Fact]
	public void Version031Info_KeepsPatchSeparateFrom030ReleaseTag()
	{
		Assert.Equal("0.3.0", Version031Info.BasedOn);
		Assert.Contains("0.3.1", Version031Info.PatchName);
		Assert.Contains("14", Version031Info.Goal);
	}

	[Fact]
	public void Branch01_BacklogAndChecklistTrackReleaseReadiness()
	{
		DevelopmentBacklog031 backlog = new();
		foreach (int branch in Enumerable.Range(1, 14))
			backlog.Add(new BranchWorkItem031(branch, $"branch_{branch}", 2, true, branch <= 12));
		ReleaseChecklist031 checklist = new();
		checklist.MarkGodotBuildPassed();
		checklist.MarkUnitTestsPassed();
		checklist.MarkNoGeneratedFoldersTracked();
		checklist.MarkVersionNotesUpdated();
		checklist.MarkSaveMigrationReady();
		Assert.True(backlog.CoversEveryBranch);
		Assert.Equal(28, backlog.TotalEstimatedHours);
		Assert.True(checklist.CanTagRelease);
	}

	[Fact]
	public void Branch02_ActionMenusAndGearComparisonAdaptToContext()
	{
		var inventoryMenu = InventoryActionMenuPlan031.ForInventoryItem(isConsumable: true, isEquipment: true, canMoveToStorage: true);
		var equipmentMenu = InventoryActionMenuPlan031.ForEquippedItem(damaged: false);
		GearComparison031 comparison = new(2, 5, 1, 1, 4f, 3f);
		Assert.Contains(InventoryActionKind031.Use, inventoryMenu.Actions);
		Assert.Contains(InventoryActionKind031.Equip, inventoryMenu.Actions);
		Assert.Equal(2, equipmentMenu.VisualRows);
		Assert.True(comparison.IsUpgrade);
		Assert.Contains("armor +3", comparison.Summary);
	}

	[Fact]
	public void Branch03_StoragePoliciesAndAuditFindCompactionNeed()
	{
		StoragePolicyPlanner031 planner = new();
		planner.AddRule(new StoragePolicyRule031("simple_medkit", StoragePolicy031.KeepInInventory, 2));
		StorageAuditReport031 audit = new(new[] { ("metal", 5, 99), ("metal", 7, 99), ("wood", 120, 99) });
		Assert.Equal(StoragePolicy031.KeepInInventory, planner.GetPolicy("simple_medkit"));
		Assert.Equal(2, planner.GetKeepAmount("simple_medkit"));
		Assert.True(audit.NeedsCompaction);
		Assert.Equal(1, audit.DuplicateItemKinds);
	}

	[Fact]
	public void Branch04_CraftPreviewAndRecipeDependenciesWork()
	{
		CraftPreview031 preview = CraftPreviewBuilder031.Build("plate", new[] { ("metal", 3, 2), ("wood", 1, 5) });
		RecipeDependencyGraph031 graph = new();
		graph.AddRequirement("advanced_plate", "plate");
		Assert.False(preview.CanCraft);
		Assert.Contains("metal", preview.MissingItems);
		Assert.False(graph.IsUnlocked("advanced_plate", Array.Empty<string>()));
		Assert.True(graph.IsUnlocked("advanced_plate", new[] { "plate" }));
	}

	[Fact]
	public void Branch05_RestAndExposureModelSurvivalPressure()
	{
		RestPlan031 rest = RestPlanner031.Plan(2.2f, currentFood: 10, currentWater: 20);
		ExposureState031 exposure = new();
		exposure.Tick(100, coldRate: 1f, radiationRate: 0.2f, resistance: 0.25f);
		Assert.True(rest.Hours > 2f);
		Assert.True(rest.IsAffordable);
		Assert.Contains("cold", exposure.GetWarnings());
	}

	[Fact]
	public void Branch06_HarvestRoutePicksSafeValuableTargetsAndRespawnScales()
	{
		var route = HarvestRoutePlanner031.PickRoute(new[]
		{
			new HarvestTarget031("safe_wood", 1, 10, 30),
			new HarvestTarget031("danger_cache", 8, 100, 10),
			new HarvestTarget031("metal", 2, 20, 40)
		}, maxDanger: 3, maxTargets: 2);
		ResourceDepletionBalancer031 balancer = new(100);
		Assert.Equal(new[] { "metal", "safe_wood" }, route.Select(x => x.Id));
		Assert.True(balancer.GetRespawnSeconds(3, 2) > 100);
	}

	[Fact]
	public void Branch07_CombatPlannerChoosesRetreatKiteOrMeleeAndAmmoSpends()
	{
		ThreatSnapshot031 highThreat = new(enemyCount: 4, highestDamage: 8, nearestDistance: 30);
		ThreatSnapshot031 farSingle = new(enemyCount: 1, highestDamage: 3, nearestDistance: 120);
		AmmoReserve031 ammo = new();
		ammo.Add("basic", 5);
		Assert.Equal("retreat", CombatEncounterPlanner031.ChooseTactic(highThreat, 90, false));
		Assert.Equal("kite", CombatEncounterPlanner031.ChooseTactic(farSingle, 90, true));
		Assert.True(ammo.TrySpend("basic", 2));
		Assert.Equal(3, ammo.Get("basic"));
	}

	[Fact]
	public void Branch08_WorldProgressionUnlocksZonesAndEventsByProgress()
	{
		var zones = new[]
		{
			new ZoneUnlock031("near", 0, 0),
			new ZoneUnlock031("factory", 2, 4),
			new ZoneUnlock031("bunker", 5, 10)
		};
		WorldEventSchedule031 schedule = new();
		schedule.Add(30, "first_wave");
		schedule.Add(90, "night_warning");
		Assert.Equal(new[] { "near", "factory" }, ZoneProgressionPlanner031.GetUnlockedZones(zones, 2, 4));
		Assert.Equal(new[] { "first_wave" }, schedule.GetDueEvents(45));
	}

	[Fact]
	public void Branch09_BaseDependenciesQueueAndComfortWork()
	{
		FacilityDependencyGraph031 graph = new();
		graph.AddDependency("radio", "generator");
		BaseProjectQueue031 queue = new();
		queue.Enqueue("storage_2");
		queue.Enqueue("workbench_2");
		BaseComfortCalculator031 comfort = new();
		Assert.False(graph.CanBuild("radio", new[] { "workbench" }));
		Assert.True(graph.CanBuild("radio", new[] { "generator" }));
		Assert.Equal("storage_2", queue.CompleteNext());
		Assert.True(comfort.Calculate(2, 1, 1, 3) > 20);
	}

	[Fact]
	public void Branch10_SaveProfileAutosaveAndIntegrityWork()
	{
		SaveProfile031 profile = new("main", "0.3.1-dev");
		profile.AddPlayTime(15);
		AutosavePolicy031 policy = new(60);
		SaveIntegrityReport031 integrity = new(Array.Empty<string>());
		Assert.Equal(15, profile.PlayMinutes);
		Assert.True(policy.ShouldAutosave(10, enteredBase: true, enteringDangerZone: false));
		Assert.True(integrity.IsValid);
	}

	[Fact]
	public void Branch11_MenuFlowAndTooltipsSelectImportantUiState()
	{
		MenuFlowNavigator031 navigator = new();
		navigator.Push(new MenuFlowStep031("inventory", "Инвентарь", true));
		navigator.Push(new MenuFlowStep031("context", "Действия", true));
		string tip = TooltipPriority031.PickBest(new[] { new TooltipPriority031("обычно", 1), new TooltipPriority031("важно", 10) });
		Assert.Equal(2, navigator.Depth);
		Assert.Equal("context", navigator.Current?.Id);
		Assert.Equal("важно", tip);
	}

	[Fact]
	public void Branch12_FeedbackIntensityAndBatchesModelGameJuice()
	{
		JuiceProfile031 profile = FeedbackIntensityPlanner031.ForDamage(40, critical: true);
		FeedbackEventBatch031 batch = new();
		batch.Add("hit");
		batch.Add("hit");
		batch.Add("loot");
		Assert.True(profile.HasStrongFeedback);
		Assert.Equal(new[] { "hit", "loot" }, batch.DrainDistinct());
		Assert.Empty(batch.Events);
	}

	[Fact]
	public void Branch13_QuestMilestonesAndTutorialFlowAdvance()
	{
		QuestMilestonePlanner031 planner = new();
		planner.Add(new QuestMilestone031("collect_wood", 3, "recipe_plank"));
		planner.Add(new QuestMilestone031("build_workbench", 10, "recipe_plate"));
		TutorialFlow031 flow = new(new[] { "move", "gather", "craft" });
		flow.CompleteCurrent();
		Assert.Single(planner.GetCompleted(5));
		Assert.Equal("gather", flow.Current);
	}

	[Fact]
	public void Branch14_QualityGateRequiresAllBranchesTestsAndChecklist()
	{
		DevelopmentBacklog031 backlog = new();
		TestCoveragePlan031 tests = new();
		foreach (int branch in Enumerable.Range(1, 14))
		{
			backlog.Add(new BranchWorkItem031(branch, $"b{branch}", 1, true, true));
			tests.AddTests(branch, 1);
		}
		ReleaseChecklist031 checklist = new();
		checklist.MarkGodotBuildPassed();
		checklist.MarkUnitTestsPassed();
		checklist.MarkNoGeneratedFoldersTracked();
		checklist.MarkVersionNotesUpdated();
		checklist.MarkSaveMigrationReady();
		QualityGateResult031 result = QualityGate031.Evaluate(backlog, tests, checklist);
		Assert.True(result.Passed);
		Assert.Empty(result.Blockers);
	}
}
