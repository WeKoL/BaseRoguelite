using Godot;

public partial class InventorySlotCard : Control
{
	[Signal]
	public delegate void ContextRequestedEventHandler(int slotIndex, Vector2 screenPosition);

	private ColorRect _backgroundRect;
	private TextureRect _iconRect;
	private ColorRect _rarityTint;
	private Label _amountLabel;
	private Label _nameLabel;

	private bool _isInitialized;
	private ItemEntry _currentEntry;
	private int _slotIndex = -1;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;

		_backgroundRect = GetNode<ColorRect>("BackgroundRect");
		_iconRect = GetNode<TextureRect>("IconRect");
		_rarityTint = GetNode<ColorRect>("RarityTint");
		_amountLabel = GetNode<Label>("AmountLabel");
		_nameLabel = GetNode<Label>("NameLabel");

		_backgroundRect.MouseFilter = MouseFilterEnum.Ignore;
		_iconRect.MouseFilter = MouseFilterEnum.Ignore;
		_rarityTint.MouseFilter = MouseFilterEnum.Ignore;
		_amountLabel.MouseFilter = MouseFilterEnum.Ignore;
		_nameLabel.MouseFilter = MouseFilterEnum.Ignore;

		_isInitialized = true;
		SetEmpty(-1);
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseButton)
			return;

		if (!mouseButton.Pressed)
			return;

		if (mouseButton.ButtonIndex != MouseButton.Right)
			return;

		if (_currentEntry == null || _currentEntry.IsEmpty() || _currentEntry.Item == null)
			return;

		Vector2 mousePos = GetViewport().GetMousePosition();
		EmitSignal(SignalName.ContextRequested, _slotIndex, mousePos);
		AcceptEvent();
	}

	public void SetEmpty(int slotIndex)
	{
		if (!_isInitialized)
			return;

		_slotIndex = slotIndex;
		_currentEntry = null;

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

	public void SetEntry(ItemEntry entry, int slotIndex)
	{
		if (!_isInitialized)
			return;

		if (entry == null || entry.IsEmpty() || entry.Item == null)
		{
			SetEmpty(slotIndex);
			return;
		}

		_slotIndex = slotIndex;
		_currentEntry = entry;

		ItemData item = entry.Item;

		_iconRect.Texture = item.Icon;
		_iconRect.Visible = item.Icon != null;

		_nameLabel.Text = item.DisplayName;
		_amountLabel.Text = $"x{entry.Amount}";

		_nameLabel.Visible = true;
		_amountLabel.Visible = true;

		TooltipText = BuildTooltip(item, entry.Amount);

		_backgroundRect.Color = GetBackgroundColor(item.Rarity);
		_rarityTint.Color = GetRarityOverlayColor(item.Rarity);
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
