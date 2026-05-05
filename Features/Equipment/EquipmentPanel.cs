using Godot;
using System;

public partial class EquipmentPanel : Control
{
	public event Action<EquipmentSlotId, Vector2> ContextRequested;

	private PlayerController _player;

	private EquipmentSlotView _capeSlot;
	private EquipmentSlotView _headSlot;
	private EquipmentSlotView _backpackSlot;
	private EquipmentSlotView _primaryWeaponSlot;
	private EquipmentSlotView _chestSlot;
	private EquipmentSlotView _secondaryWeaponSlot;
	private EquipmentSlotView _bootsSlot;

	public override void _Ready()
	{
		_capeSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/CapeSlot");
		_headSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/HeadSlot");
		_backpackSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/BackpackSlot");

		_primaryWeaponSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/PrimaryWeaponSlot");
		_chestSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/ChestSlot");
		_secondaryWeaponSlot = GetNode<EquipmentSlotView>("PanelRoot/TopGrid/SecondaryWeaponSlot");

		_bootsSlot = GetNode<EquipmentSlotView>("PanelRoot/BottomRow/BootsSlot");

		_capeSlot.ContextRequested += OnSlotContextRequested;
		_headSlot.ContextRequested += OnSlotContextRequested;
		_backpackSlot.ContextRequested += OnSlotContextRequested;
		_primaryWeaponSlot.ContextRequested += OnSlotContextRequested;
		_chestSlot.ContextRequested += OnSlotContextRequested;
		_secondaryWeaponSlot.ContextRequested += OnSlotContextRequested;
		_bootsSlot.ContextRequested += OnSlotContextRequested;

		RefreshView();
	}

	public void BindPlayer(PlayerController player)
	{
		_player = player;
		RefreshView();
	}

	public void RefreshView()
	{
		RefreshSlot(_capeSlot, "Плащ", EquipmentSlotId.Cape);
		RefreshSlot(_headSlot, "Шлем", EquipmentSlotId.Head);
		RefreshSlot(_backpackSlot, "Рюкзак", EquipmentSlotId.Backpack);

		RefreshSlot(_primaryWeaponSlot, "Оружие 1", EquipmentSlotId.PrimaryWeapon);
		RefreshSlot(_chestSlot, "Броня", EquipmentSlotId.Chest);
		RefreshSlot(_secondaryWeaponSlot, "Оружие 2", EquipmentSlotId.SecondaryWeapon);

		RefreshSlot(_bootsSlot, "Ботинки", EquipmentSlotId.Boots);
	}

	private void RefreshSlot(EquipmentSlotView slotView, string slotLabel, EquipmentSlotId slotId)
	{
		if (slotView == null)
			return;

		if (_player == null)
		{
			slotView.SetEmpty(slotId, slotLabel);
			return;
		}

		EquippedItemViewData viewData = _player.GetEquippedItemViewData(slotId);
		if (viewData == null)
		{
			slotView.SetEmpty(slotId, slotLabel);
			return;
		}

		slotView.SetItem(slotId, slotLabel, viewData);
	}

	private void OnSlotContextRequested(EquipmentSlotId slotId, Vector2 screenPosition)
	{
		ContextRequested?.Invoke(slotId, screenPosition);
	}
}
