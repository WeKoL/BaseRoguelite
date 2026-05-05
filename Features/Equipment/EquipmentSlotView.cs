using Godot;
using System;

public partial class EquipmentSlotView : Control
{
	public event Action<EquipmentSlotId, Vector2> ContextRequested;

	private ColorRect _backgroundRect;
	private TextureRect _iconRect;
	private Label _slotLabel;
	private Label _nameLabel;
	private Label _durabilityLabel;
	private Label _emptyLabel;

	private EquipmentSlotId _slotId;
	private string _slotDisplayName = string.Empty;
	private EquippedItemViewData _currentData;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;

		_backgroundRect = GetNode<ColorRect>("BackgroundRect");
		_iconRect = GetNode<TextureRect>("IconRect");
		_slotLabel = GetNode<Label>("SlotLabel");
		_nameLabel = GetNode<Label>("NameLabel");
		_durabilityLabel = GetNode<Label>("DurabilityLabel");
		_emptyLabel = GetNode<Label>("EmptyLabel");

		_backgroundRect.MouseFilter = MouseFilterEnum.Ignore;
		_iconRect.MouseFilter = MouseFilterEnum.Ignore;
		_slotLabel.MouseFilter = MouseFilterEnum.Ignore;
		_nameLabel.MouseFilter = MouseFilterEnum.Ignore;
		_durabilityLabel.MouseFilter = MouseFilterEnum.Ignore;
		_emptyLabel.MouseFilter = MouseFilterEnum.Ignore;

		SetEmpty(EquipmentSlotId.Head, "Слот");
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseButton)
			return;

		if (!mouseButton.Pressed)
			return;

		if (mouseButton.ButtonIndex != MouseButton.Right)
			return;

		if (_currentData == null)
			return;

		Vector2 mousePos = GetViewport().GetMousePosition();
		ContextRequested?.Invoke(_slotId, mousePos);
		AcceptEvent();
	}

	public void SetEmpty(EquipmentSlotId slotId, string slotLabel)
	{
		_slotId = slotId;
		_slotDisplayName = slotLabel ?? string.Empty;
		_currentData = null;

		_slotLabel.Text = _slotDisplayName;
		_backgroundRect.Color = new Color(0.18f, 0.18f, 0.18f, 0.90f);

		_iconRect.Texture = null;
		_iconRect.Visible = false;

		_nameLabel.Text = string.Empty;
		_nameLabel.Visible = false;

		_durabilityLabel.Text = string.Empty;
		_durabilityLabel.Visible = false;

		_emptyLabel.Text = "пусто";
		_emptyLabel.Visible = true;

		TooltipText = _slotDisplayName;
	}

	public void SetItem(EquipmentSlotId slotId, string slotLabel, EquippedItemViewData data)
	{
		if (data == null)
		{
			SetEmpty(slotId, slotLabel);
			return;
		}

		_slotId = slotId;
		_slotDisplayName = slotLabel ?? string.Empty;
		_currentData = data;

		_slotLabel.Text = _slotDisplayName;
		_backgroundRect.Color = new Color(0.24f, 0.24f, 0.24f, 0.95f);

		_iconRect.Texture = data.Icon;
		_iconRect.Visible = data.Icon != null;

		_nameLabel.Text = data.DisplayName;
		_nameLabel.Visible = true;

		_durabilityLabel.Text = $"{data.CurrentDurability}/{data.MaxDurability}";
		_durabilityLabel.Visible = true;

		_emptyLabel.Visible = false;

		TooltipText =
			$"{_slotDisplayName}\n" +
			$"{data.DisplayName}\n" +
			$"Прочность: {data.CurrentDurability}/{data.MaxDurability}";
	}
}
