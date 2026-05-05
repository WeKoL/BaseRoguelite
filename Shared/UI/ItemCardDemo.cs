using Godot;

public partial class ItemCardDemo : Control
{
	[Export]
	public PackedScene ItemCardScene { get; set; }

	private GridContainer _cardsGrid;

	public override void _Ready()
	{
		_cardsGrid = GetNode<GridContainer>("RootMargin/RootVBox/Scroll/ContentMargin/CardsGrid");
		CallDeferred(nameof(BuildDemoCards));
	}

	private void BuildDemoCards()
	{
		ClearCards();

		if (ItemCardScene == null)
		{
			GD.PushError("ItemCardDemo: не назначена сцена ItemCardScene.");
			return;
		}

		AddDemoCard(CreateItem(
			id: "rock",
			displayName: "Камень",
			description: "Базовый природный материал.",
			rarity: ItemRarity.Common,
			category: ItemCategory.Material,
			usageHint: "Используется в строительстве и крафте.",
			weight: 1.0f
		), 16);

		AddDemoCard(CreateItem(
			id: "wooden_plank",
			displayName: "Доски",
			description: "Обычные доски для строительства и ремонта.",
			rarity: ItemRarity.Common,
			category: ItemCategory.Material,
			usageHint: "Используются в строительстве и ремонте базы.",
			weight: 1.0f
		), 22);

		AddDemoCard(CreateItem(
			id: "metal",
			displayName: "Металл",
			description: "Базовый металлический материал для крафта и укреплений.",
			rarity: ItemRarity.Common,
			category: ItemCategory.Material,
			usageHint: "Нужен для крафта, укреплений и ремонта.",
			weight: 1.0f
		), 13);
	}

	private void AddDemoCard(ItemData item, int amount)
	{
		Node node = ItemCardScene.Instantiate();

		if (node is not ItemCard card)
		{
			GD.PushError($"ItemCardDemo: сцена ItemCardScene должна инстанцироваться как ItemCard, а пришёл {node.GetType().Name}.");
			node.QueueFree();
			return;
		}

		_cardsGrid.AddChild(card);

		ItemEntry entry = new ItemEntry(item, amount);
		card.SetItemEntry(entry);
	}

	private void ClearCards()
	{
		foreach (Node child in _cardsGrid.GetChildren())
			child.QueueFree();
	}

	private ItemData CreateItem(
		string id,
		string displayName,
		string description,
		ItemRarity rarity,
		ItemCategory category,
		string usageHint,
		float weight)
	{
		return new ItemData
		{
			Id = id,
			DisplayName = displayName,
			Description = description,
			Rarity = rarity,
			Category = category,
			UsageHint = usageHint,
			Weight = weight,
			MaxStackSize = 99,
			Icon = null
		};
	}
}
