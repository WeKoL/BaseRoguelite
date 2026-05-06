using System;
using System.Collections.Generic;
using System.Linq;

public sealed class GameCore040
{
	public ItemRegistry040 Items { get; }
	public InventoryCore040 Inventory { get; }
	public EquipmentCore040 Equipment { get; }
	public StorageCore040 Storage { get; }
	public BaseCore040 Base { get; }
	public RecipeBook040 Recipes { get; }
	public CraftingCore040 Crafting { get; }
	public PlayerVitals040 Player { get; }
	public WorldCore040 World { get; }
	public ExpeditionCore040 Expedition { get; }
	public QuestTracker040 Quests { get; }
	public FeedbackBus040 Feedback { get; }

	public GameCore040()
	{
		Items = ItemRegistry040.CreateDefault();
		Inventory = new InventoryCore040(Items, 24, 50f);
		Equipment = new EquipmentCore040(Items);
		Storage = new StorageCore040(80, 3000);
		Base = new BaseCore040();
		Base.SetFacility(BaseFacility040.Storage, 1);
		Base.SetFacility(BaseFacility040.Workbench, 1);
		Base.SetFacility(BaseFacility040.Medbay, 1);
		Recipes = RecipeBook040.CreateDefault();
		Crafting = new CraftingCore040(Recipes, Base);
		Player = new PlayerVitals040();
		World = WorldCore040.CreateDefault();
		Expedition = new ExpeditionCore040();
		Quests = QuestTracker040.CreateTutorial();
		Feedback = new FeedbackBus040();
	}

	public void StartNewGame()
	{
		Inventory.Add("knife", 1);
		Inventory.Add("food", 2);
		Inventory.Add("water", 2);
		Storage.Add("wood", 5);
		Storage.Add("stone", 5);
		Storage.Add("metal", 5);
		Storage.Add("cloth", 3);
		Storage.Add("herb", 2);
	}

	public CoreStepReport040 RunCoreStep(int seconds)
	{
		SurvivalTickResult040 survival = Player.Tick(seconds, running: false, inBase: Expedition.CurrentZone == WorldZone040.Base);
		if (survival.HasPressure) Feedback.Push(FeedbackKind040.Info, "Выживание требует внимания", 1f);
		if (!Player.IsAlive) Feedback.Push(FeedbackKind040.Death, "Экспедиция провалена", 8f);
		return new CoreStepReport040(survival, Storage.Audit(), Feedback.Drain());
	}
}

public sealed class CoreStepReport040
{
	public SurvivalTickResult040 Survival { get; }
	public StorageAudit040 StorageAudit { get; }
	public IReadOnlyList<FeedbackEvent040> FeedbackEvents { get; }
	public CoreStepReport040(SurvivalTickResult040 survival, StorageAudit040 storageAudit, IReadOnlyList<FeedbackEvent040> feedbackEvents)
	{
		Survival = survival;
		StorageAudit = storageAudit;
		FeedbackEvents = feedbackEvents ?? Array.Empty<FeedbackEvent040>();
	}
}
