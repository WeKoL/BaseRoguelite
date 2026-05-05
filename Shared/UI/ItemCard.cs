using Godot;

public partial class ItemCard : Control
{
	private const int ContextActionInfo = 1;

	private ColorRect _backgroundRect;
	private TextureRect _iconRect;
	private ColorRect _rarityTint;
	private Label _amountLabel;
	private Label _nameLabel;

	private PopupMenu _contextMenu;
	private AcceptDialog _infoDialog;
	private Label _infoContent;

	private ItemEntry _itemEntry;
	private bool _isInitialized;

	public override void _Ready()
	{
		_backgroundRect = GetNode<ColorRect>("BackgroundRect");
		_iconRect = GetNode<TextureRect>("IconRect");
		_rarityTint = GetNode<ColorRect>("RarityTint");
		_amountLabel = GetNode<Label>("AmountLabel");
		_nameLabel = GetNode<Label>("NameLabel");

		_contextMenu = GetNode<PopupMenu>("ContextMenu");
		_infoDialog = GetNode<AcceptDialog>("InfoDialog");
		_infoContent = GetNode<Label>("InfoDialog/InfoContent");

		_contextMenu.IdPressed += OnContextMenuIdPressed;

		_isInitialized = true;

		if (_itemEntry != null)
			ApplyEntryToView();
		else
			ClearCard();
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseButton)
			return;

		if (mouseButton.ButtonIndex != MouseButton.Right || !mouseButton.Pressed)
			return;

		if (_itemEntry == null || _itemEntry.IsEmpty() || _itemEntry.Item == null)
			return;

		ShowContextMenu();
		AcceptEvent();
	}

	public void SetItemEntry(ItemEntry entry)
	{
		_itemEntry = entry;

		if (!_isInitialized)
			return;

		ApplyEntryToView();
	}

	public void ClearCard()
	{
		_itemEntry = null;

		if (!_isInitialized)
			return;

		_backgroundRect.Color = new Color(0.18f, 0.18f, 0.18f, 0.90f);
		_iconRect.Texture = null;
		_iconRect.Visible = false;

		_rarityTint.Color = new Color(1f, 1f, 1f, 0f);

		_nameLabel.Text = "";
		_amountLabel.Text = "";

		_nameLabel.Visible = false;
		_amountLabel.Visible = false;

		TooltipText = "";
	}

	private void ApplyEntryToView()
	{
		if (_itemEntry == null || _itemEntry.IsEmpty() || _itemEntry.Item == null)
		{
			ClearCard();
			return;
		}

		ItemData item = _itemEntry.Item;

		_iconRect.Texture = item.Icon;
		_iconRect.Visible = item.Icon != null;

		_nameLabel.Text = item.DisplayName;
		_amountLabel.Text = $"x{_itemEntry.Amount}";

		_nameLabel.Visible = true;
		_amountLabel.Visible = true;

		TooltipText = BuildTooltip(item, _itemEntry.Amount);

		_backgroundRect.Color = GetBackgroundColor(item.Rarity);
		_rarityTint.Color = GetRarityOverlayColor(item.Rarity);
	}

	private void ShowContextMenu()
	{
		_contextMenu.Clear();
		_contextMenu.AddItem("Информация", ContextActionInfo);

		Vector2 mousePos = GetViewport().GetMousePosition();
		_contextMenu.Position = new Vector2I((int)mousePos.X, (int)mousePos.Y);
		_contextMenu.ResetSize();
		_contextMenu.Popup();
	}

	private void OnContextMenuIdPressed(long id)
	{
		if (id != ContextActionInfo)
			return;

		ShowInfoDialog();
	}

	private void ShowInfoDialog()
	{
		if (_itemEntry == null || _itemEntry.IsEmpty() || _itemEntry.Item == null)
			return;

		ItemData item = _itemEntry.Item;
		_infoDialog.Title = item.DisplayName;
		_infoContent.Text = ItemTextFormatter.BuildFullInfo(item, _itemEntry.Amount);
		_infoDialog.PopupCentered(new Vector2I(420, 260));
	}

	private static string BuildInfoText(ItemData item, int amount)
	{
		string description = string.IsNullOrWhiteSpace(item.Description)
			? "Без описания."
			: item.Description;

		string usageHint = string.IsNullOrWhiteSpace(item.UsageHint)
			? string.Empty
			: $"\nПодсказка: {item.UsageHint}";

		return
			$"Название: {item.DisplayName}\n" +
			$"Количество: {amount}\n" +
			$"Категория: {item.GetDisplayCategory()}\n" +
			$"Редкость: {GetRarityText(item.Rarity)}\n" +
			$"Вес одной единицы: {item.Weight:0.##}\n\n" +
			$"{description}{usageHint}";
	}

	private static string GetRarityText(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => "Обычный",
			ItemRarity.Rare => "Редкий",
			ItemRarity.SuperRare => "Очень редкий",
			ItemRarity.Epic => "Эпический",
			ItemRarity.Legendary => "Легендарный",
			_ => "Неизвестно"
		};
	}

	private static Color GetBackgroundColor(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => new Color(0.24f, 0.24f, 0.24f, 0.95f),
			ItemRarity.Rare => new Color(0.16f, 0.26f, 0.16f, 0.95f),
			ItemRarity.SuperRare => new Color(0.14f, 0.20f, 0.34f, 0.95f),
			ItemRarity.Epic => new Color(0.24f, 0.16f, 0.30f, 0.95f),
			ItemRarity.Legendary => new Color(0.36f, 0.28f, 0.10f, 0.95f),
			_ => new Color(0.24f, 0.24f, 0.24f, 0.95f)
		};
	}

	private static Color GetRarityOverlayColor(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => new Color(1f, 1f, 1f, 0.04f),
			ItemRarity.Rare => new Color(0.25f, 0.85f, 0.25f, 0.12f),
			ItemRarity.SuperRare => new Color(0.25f, 0.55f, 1f, 0.14f),
			ItemRarity.Epic => new Color(0.72f, 0.35f, 0.95f, 0.14f),
			ItemRarity.Legendary => new Color(1f, 0.78f, 0.20f, 0.16f),
			_ => new Color(1f, 1f, 1f, 0.04f)
		};
	}

	private static string BuildTooltip(ItemData item, int amount)
	{
		string description = string.IsNullOrWhiteSpace(item.Description)
			? "Без описания."
			: item.Description;

		return
			$"{item.DisplayName}\n" +
			$"Количество: {amount}\n" +
			$"Вес одной единицы: {item.Weight:0.##}\n\n" +
			$"{description}";
	}
}
