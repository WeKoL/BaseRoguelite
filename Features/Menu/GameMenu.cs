using Godot;
using System;

public partial class GameMenu : Control
{
	private enum ItemContextSourceKind
	{
		None = 0,
		InventorySlot = 1,
		EquipmentSlot = 2
	}

	private const int InventoryContextMenuReservedRows = 3;
	private const int EquipmentContextMenuReservedRows = 2;
	private const float SharedContextMenuButtonMinWidth = 120.0f;
	private const float SharedContextMenuHorizontalPadding = 12.0f;
	private const float SharedContextMenuVerticalPadding = 12.0f;
	private const float SharedContextMenuButtonSeparation = 4.0f;
	private const float SharedContextMenuButtonFallbackHeight = 31.0f;

	private readonly PackedScene _inventorySlotCardScene =
		GD.Load<PackedScene>("res://Features/Inventory/InventorySlotCard.tscn");

	private readonly PackedScene _worldPickupScene =
		GD.Load<PackedScene>("res://Features/World/Pickups/WorldPickup.tscn");

	private readonly MenuState _menuState = new();

	private PlayerController _player;
	private BaseRoot _baseRoot;

	public event Action<ItemData, int, int> InventoryItemDropped;

	private Button _inventoryTabButton;
	private Button _baseTabButton;
	private Button _journalTabButton;
	private Button _rootCloseButton;
	private Button _subCloseButton;

	private Control _rootView;
	private Control _subView;
	private Control _inventoryPanel;
	private Control _basePanel;
	private Control _journalPanel;

	private Label _inventoryTitleLabel;
	private GridContainer _inventorySlotsGrid;
	private Label _carryWeightLabel;
	private ColorRect _carryBarFill;
	private EquipmentPanel _equipmentPanel;

	private Label _baseInfoLabel;
	private Label _journalTextLabel;
	private Label _subTitleLabel;

	private Button _storageButton;
	private Button _craftButton;
	private Button _healButton;

	private StorageSubpanel _storageSubpanel;
	private Control _craftSubpanel;
	private Control _healSubpanel;
	private Label _craftInfoLabel;
	private Button _craftMedkitButton;
	private Button _craftPlateButton;
	private Button _craftRepairKitButton;
	private Button _craftGeneratorPartButton;
	private Button _craftRationButton;
	private Button _craftWaterFilterButton;
	private Button _craftToolKitButton;
	private Button _craftAmmoPackButton;
	private Button _craftCampBeaconButton;
	private Button _upgradeStorageButton;
	private Button _upgradeWorkbenchButton;
	private Button _upgradeMedbayButton;
	private Button _upgradeGeneratorButton;
	private Button _upgradeRadioTowerButton;
	private Button _upgradeDefensiveWallsButton;
	private Label _healInfoLabel;
	private Button _healNowButton;

	private PanelContainer _sharedContextMenu;
	private Button _ctxEquipButton;
	private Button _ctxUnequipButton;
	private Button _ctxDropButton;
	private Button _ctxInfoButton;

	private AcceptDialog _sharedInfoDialog;
	private Label _sharedInfoContent;

	private ItemContextSourceKind _contextSourceKind = ItemContextSourceKind.None;
	private int _contextInventorySlotIndex = -1;
	private EquipmentSlotId _contextEquipmentSlotId = EquipmentSlotId.Head;

	public bool IsOpen => _menuState.IsOpen;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		MouseFilter = MouseFilterEnum.Stop;

		_inventoryTabButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootHeader/Tabs/InventoryTabButton");
		_baseTabButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootHeader/Tabs/BaseTabButton");
		_journalTabButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootHeader/Tabs/JournalTabButton");
		_rootCloseButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootHeader/RootCloseButton");
		_subCloseButton = GetNode<Button>("Panel/RootMargin/ViewHost/SubView/SubHeader/SubCloseButton");

		_rootView = GetNode<Control>("Panel/RootMargin/ViewHost/RootView");
		_subView = GetNode<Control>("Panel/RootMargin/ViewHost/SubView");
		_inventoryPanel = GetNode<Control>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel");
		_basePanel = GetNode<Control>("Panel/RootMargin/ViewHost/RootView/RootContent/BasePanel");
		_journalPanel = GetNode<Control>("Panel/RootMargin/ViewHost/RootView/RootContent/JournalPanel");

		_inventoryTitleLabel = GetNode<Label>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel/InventoryRoot/InventoryTitleLabel");
		_inventorySlotsGrid = GetNode<GridContainer>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel/InventoryRoot/InventoryBody/InventoryLeft/SlotsGrid");
		_carryWeightLabel = GetNode<Label>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel/InventoryRoot/InventoryBody/InventoryLeft/CarryBlock/CarryWeightLabel");
		_carryBarFill = GetNode<ColorRect>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel/InventoryRoot/InventoryBody/InventoryLeft/CarryBlock/CarryBarBackground/CarryBarFill");
		_equipmentPanel = GetNode<EquipmentPanel>("Panel/RootMargin/ViewHost/RootView/RootContent/InventoryPanel/InventoryRoot/InventoryBody/EquipmentPanel");

		_baseInfoLabel = GetNode<Label>("Panel/RootMargin/ViewHost/RootView/RootContent/BasePanel/BaseInfoLabel");
		_journalTextLabel = GetNode<Label>("Panel/RootMargin/ViewHost/RootView/RootContent/JournalPanel/JournalTextLabel");
		_subTitleLabel = GetNode<Label>("Panel/RootMargin/ViewHost/SubView/SubHeader/SubTitleLabel");

		_storageButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootContent/BasePanel/BaseActionCards/StorageButton");
		_craftButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootContent/BasePanel/BaseActionCards/CraftButton");
		_healButton = GetNode<Button>("Panel/RootMargin/ViewHost/RootView/RootContent/BasePanel/BaseActionCards/HealButton");

		_storageSubpanel = GetNode<StorageSubpanel>("Panel/RootMargin/ViewHost/SubView/SubContent/StorageSubpanel");
		_craftSubpanel = GetNode<Control>("Panel/RootMargin/ViewHost/SubView/SubContent/CraftSubpanel");
		_healSubpanel = GetNode<Control>("Panel/RootMargin/ViewHost/SubView/SubContent/HealSubpanel");
		BuildVisibleCraftAndHealPanels();

		_sharedContextMenu = GetNode<PanelContainer>("SharedContextMenu");
		_ctxEquipButton = GetNode<Button>("SharedContextMenu/MenuMargin/MenuButtons/EquipButton");
		_ctxUnequipButton = GetNode<Button>("SharedContextMenu/MenuMargin/MenuButtons/UnequipButton");
		_ctxDropButton = GetNode<Button>("SharedContextMenu/MenuMargin/MenuButtons/DropButton");
		_ctxInfoButton = GetNode<Button>("SharedContextMenu/MenuMargin/MenuButtons/InfoButton");

		_sharedInfoDialog = GetNode<AcceptDialog>("SharedItemInfoDialog");
		_sharedInfoContent = GetNode<Label>("SharedItemInfoDialog/InfoContent");

		_inventoryTabButton.Pressed += () => SelectRootSection(MenuSection.Inventory);
		_baseTabButton.Pressed += () => SelectRootSection(MenuSection.Base);
		_journalTabButton.Pressed += () => SelectRootSection(MenuSection.Journal);

		_rootCloseButton.Pressed += CloseMenu;
		_subCloseButton.Pressed += CloseSubView;

		_storageButton.Pressed += () => OnBaseActionPressed(BaseActionId.Storage);
		_craftButton.Pressed += () => OnBaseActionPressed(BaseActionId.Craft);
		_healButton.Pressed += () => OnBaseActionPressed(BaseActionId.Heal);

		_equipmentPanel.ContextRequested += OnEquipmentContextRequested;

		_ctxEquipButton.Pressed += OnSharedEquipPressed;
		_ctxUnequipButton.Pressed += OnSharedUnequipPressed;
		_ctxDropButton.Pressed += OnSharedDropPressed;
		_ctxInfoButton.Pressed += OnSharedInfoPressed;
		if (_craftMedkitButton != null)
			_craftMedkitButton.Pressed += () => TryCraftById("craft_simple_medkit");
		if (_craftPlateButton != null)
			_craftPlateButton.Pressed += () => TryCraftById("craft_reinforced_plate");
		if (_craftRepairKitButton != null)
			_craftRepairKitButton.Pressed += () => TryCraftById("craft_repair_kit");
		if (_craftGeneratorPartButton != null)
			_craftGeneratorPartButton.Pressed += () => TryCraftById("craft_generator_part");
		if (_craftRationButton != null)
			_craftRationButton.Pressed += () => TryCraftById("craft_field_ration");
		if (_craftWaterFilterButton != null)
			_craftWaterFilterButton.Pressed += () => TryCraftById("craft_water_filter");
		if (_craftToolKitButton != null)
			_craftToolKitButton.Pressed += () => TryCraftById("craft_tool_kit");
		if (_craftAmmoPackButton != null)
			_craftAmmoPackButton.Pressed += () => TryCraftById("craft_ammo_pack");
		if (_craftCampBeaconButton != null)
			_craftCampBeaconButton.Pressed += () => TryCraftById("craft_camp_beacon");
		if (_upgradeStorageButton != null)
			_upgradeStorageButton.Pressed += () => TryUpgradeById("storage_box");
		if (_upgradeWorkbenchButton != null)
			_upgradeWorkbenchButton.Pressed += () => TryUpgradeById("workbench");
		if (_upgradeMedbayButton != null)
			_upgradeMedbayButton.Pressed += () => TryUpgradeById("medical_station");
		if (_upgradeGeneratorButton != null)
			_upgradeGeneratorButton.Pressed += () => TryUpgradeById("generator");
		if (_upgradeRadioTowerButton != null)
			_upgradeRadioTowerButton.Pressed += () => TryUpgradeById("radio_tower");
		if (_upgradeDefensiveWallsButton != null)
			_upgradeDefensiveWallsButton.Pressed += () => TryUpgradeById("defensive_walls");
		if (_healNowButton != null)
			_healNowButton.Pressed += TryHealPlayerAtBase;

		HideSharedContextMenu();

		Hide();
		Refresh();
	}

	public override void _Input(InputEvent @event)
	{
		if (!Visible || _sharedContextMenu == null || !_sharedContextMenu.Visible)
			return;

		if (@event is not InputEventMouseButton mouseButton)
			return;

		if (!mouseButton.Pressed || mouseButton.ButtonIndex != MouseButton.Left)
			return;

		if (IsPointInsideSharedContextMenu(GetViewport().GetMousePosition()))
			return;

		HideSharedContextMenu();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Visible)
			return;

		if (@event.IsActionPressed("ui_cancel"))
		{
			HideSharedContextMenu();
			HandleBackAction();
			GetViewport().SetInputAsHandled();
			return;
		}

		if (@event.IsActionPressed("open_menu"))
		{
			HideSharedContextMenu();
			CloseMenu();
			GetViewport().SetInputAsHandled();
		}
	}

	public void BindPlayer(PlayerController player)
	{
		_player = player;
		_equipmentPanel?.BindPlayer(player);
		Refresh();
	}

	public void BindBase(BaseRoot baseRoot)
	{
		_baseRoot = baseRoot;
		RefreshStorageData();
	}

	public void OpenDefault()
	{
		_menuState.OpenDefault();
		OpenMenuCommon();
	}

	public void OpenWithContext(GameMenuContext context)
	{
		context ??= new GameMenuContext();
		_menuState.OpenWithContext(context.Section, context.FocusedBaseAction);
		OpenMenuCommon();
	}

	public void CloseMenu()
	{
		HideSharedContextMenu();
		_menuState.CloseMenu();
		Hide();
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
	}

	public void ToggleMenu()
	{
		if (_menuState.IsOpen)
			CloseMenu();
		else
			OpenDefault();
	}

	public bool HandleBackAction()
	{
		HideSharedContextMenu();

		MenuBackActionResult result = _menuState.HandleBackAction();

		if (result == MenuBackActionResult.None)
			return false;

		if (result == MenuBackActionResult.ClosedSubView)
		{
			Refresh();
			return true;
		}

		if (result == MenuBackActionResult.ClosedMenu)
		{
			Hide();
			GetTree().Paused = false;
			Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
			Refresh();
			return true;
		}

		return false;
	}

	private void OpenMenuCommon()
	{
		Refresh();
		Show();
		GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	private bool CanUseBaseActions()
	{
		return _player != null && _player.IsInsideBase;
	}

	private void SelectRootSection(MenuSection section)
	{
		HideSharedContextMenu();
		_menuState.SelectRootSection(section);
		Refresh();
	}

	private void OnBaseActionPressed(BaseActionId actionId)
	{
		if (!_menuState.TryOpenBaseAction(actionId, CanUseBaseActions()))
			return;

		Refresh();
	}

	private void CloseSubView()
	{
		HideSharedContextMenu();
		_menuState.CloseSubView();
		Refresh();
	}

	private void Refresh()
	{
		bool onBase = _player != null && _player.IsInsideBase;
		bool canUseBaseActions = CanUseBaseActions();

		_rootView.Visible = !_menuState.IsSubViewOpen;
		_subView.Visible = _menuState.IsSubViewOpen;

		_inventoryPanel.Visible = _menuState.CurrentRootSection == MenuSection.Inventory;
		_basePanel.Visible = _menuState.CurrentRootSection == MenuSection.Base;
		_journalPanel.Visible = _menuState.CurrentRootSection == MenuSection.Journal;

		_inventoryTabButton.Disabled = _menuState.CurrentRootSection == MenuSection.Inventory;
		_baseTabButton.Disabled = _menuState.CurrentRootSection == MenuSection.Base;
		_journalTabButton.Disabled = _menuState.CurrentRootSection == MenuSection.Journal;

		_storageButton.Disabled = !canUseBaseActions;
		_craftButton.Disabled = !canUseBaseActions;
		_healButton.Disabled = !canUseBaseActions;

		_inventoryTitleLabel.Text = "Инвентарь";
		_journalTextLabel.Text = BuildJournalText();
		_baseInfoLabel.Text = BuildBaseInfoText(onBase);
		RefreshCraftAndHealPanelText();

		RefreshInventoryView();
		RefreshStorageData();
		RefreshSubView();
	}

	private void RefreshInventoryView()
	{
		foreach (Node child in _inventorySlotsGrid.GetChildren())
			child.QueueFree();

		PlayerInventoryState inventory = _player?.Inventory;
		int slotCount = inventory?.MaxSlots ?? 16;

		for (int i = 0; i < slotCount; i++)
		{
			InventorySlotCard slotCard = _inventorySlotCardScene.Instantiate<InventorySlotCard>();
			slotCard.ContextRequested += OnInventorySlotContextRequested;
			_inventorySlotsGrid.AddChild(slotCard);

			ItemEntry entry = inventory?.GetEntryAt(i);

			if (entry != null && !entry.IsEmpty())
				slotCard.SetEntry(entry, i);
			else
				slotCard.SetEmpty(i);
		}

		float currentWeight = inventory?.GetCurrentWeight() ?? 0.0f;
		float maxWeight = inventory?.MaxCarryWeight ?? 60.0f;
		float ratio = inventory?.GetWeightFillRatio() ?? 0.0f;

		_carryWeightLabel.Text = $"{currentWeight:0.0} / {maxWeight:0.0} кг";
		_carryBarFill.AnchorRight = ratio;
		_carryBarFill.OffsetRight = 0;

		_equipmentPanel?.RefreshView();
	}

	private void OnInventorySlotContextRequested(int slotIndex, Vector2 screenPosition)
	{
		if (_player == null)
			return;

		ItemEntry entry = _player.Inventory?.GetEntryAt(slotIndex);
		if (entry == null || entry.IsEmpty() || entry.Item == null)
			return;

		_contextSourceKind = ItemContextSourceKind.InventorySlot;
		_contextInventorySlotIndex = slotIndex;

		bool canUse = entry.Item.CanUse();
		bool canEquip = entry.Item.CanEquip();

		_ctxEquipButton.Text = canUse ? "Использовать" : "Надеть";
		_ctxEquipButton.Visible = canUse || canEquip;
		_ctxUnequipButton.Visible = false;
		_ctxDropButton.Visible = true;
		_ctxInfoButton.Visible = true;

		ShowSharedContextMenu(screenPosition, InventoryContextMenuReservedRows);
	}

	private void OnEquipmentContextRequested(EquipmentSlotId slotId, Vector2 screenPosition)
	{
		if (_player == null)
			return;

		EquippedItemViewData equipped = _player.GetEquippedItemViewData(slotId);
		if (equipped == null)
			return;

		_contextSourceKind = ItemContextSourceKind.EquipmentSlot;
		_contextEquipmentSlotId = slotId;

		_ctxEquipButton.Visible = false;
		_ctxUnequipButton.Visible = true;
		_ctxDropButton.Visible = false;
		_ctxInfoButton.Visible = true;

		ShowSharedContextMenu(screenPosition, EquipmentContextMenuReservedRows);
	}

	private void ShowSharedContextMenu(Vector2 screenPosition, int reservedRows)
	{
		_sharedContextMenu.Position = screenPosition;
		ResizeSharedContextMenu(reservedRows);
		_sharedContextMenu.Show();
		_sharedContextMenu.ResetSize();
	}

	private void ResizeSharedContextMenu(int reservedRows)
	{
		int rowCount = Math.Max(1, reservedRows);
		float buttonHeight = GetSharedContextMenuButtonHeight();
		float menuWidth = SharedContextMenuButtonMinWidth + SharedContextMenuHorizontalPadding;
		float menuHeight = SharedContextMenuVerticalPadding
			+ rowCount * buttonHeight
			+ (rowCount - 1) * SharedContextMenuButtonSeparation;

		_sharedContextMenu.CustomMinimumSize = new Vector2(menuWidth, menuHeight);
		_sharedContextMenu.Size = _sharedContextMenu.CustomMinimumSize;
	}

	private float GetSharedContextMenuButtonHeight()
	{
		float height = SharedContextMenuButtonFallbackHeight;
		height = Math.Max(height, _ctxEquipButton.GetCombinedMinimumSize().Y);
		height = Math.Max(height, _ctxUnequipButton.GetCombinedMinimumSize().Y);
		height = Math.Max(height, _ctxDropButton.GetCombinedMinimumSize().Y);
		height = Math.Max(height, _ctxInfoButton.GetCombinedMinimumSize().Y);

		return height;
	}

	private bool IsPointInsideSharedContextMenu(Vector2 viewportPosition)
	{
		if (_sharedContextMenu == null || !_sharedContextMenu.Visible)
			return false;

		return _sharedContextMenu.GetGlobalRect().HasPoint(viewportPosition);
	}

	private void HideSharedContextMenu(bool clearContext = true)
	{
		_sharedContextMenu?.Hide();

		if (!clearContext)
			return;

		ClearSharedContext();
	}

	private void ClearSharedContext()
	{
		_contextSourceKind = ItemContextSourceKind.None;
		_contextInventorySlotIndex = -1;
	}

	private void OnSharedEquipPressed()
	{
		HideSharedContextMenu(clearContext: false);

		if (_player == null || _contextSourceKind != ItemContextSourceKind.InventorySlot)
		{
			ClearSharedContext();
			return;
		}

		int slotIndex = _contextInventorySlotIndex;
		ItemEntry entry = _player.Inventory?.GetEntryAt(slotIndex);
		ClearSharedContext();

		bool ok = entry != null && !entry.IsEmpty() && entry.Item != null && entry.Item.CanUse()
			? _player.TryUseInventorySlot(slotIndex)
			: _player.TryEquipFromInventorySlot(slotIndex);
		if (ok) Refresh();
	}

	private void OnSharedUnequipPressed()
	{
		HideSharedContextMenu(clearContext: false);

		if (_player == null || _contextSourceKind != ItemContextSourceKind.EquipmentSlot)
		{
			ClearSharedContext();
			return;
		}

		EquipmentSlotId slotId = _contextEquipmentSlotId;
		ClearSharedContext();

		if (_player.TryUnequip(slotId))
			Refresh();
	}

	private void OnSharedDropPressed()
	{
		HideSharedContextMenu(clearContext: false);

		if (_player == null || _contextSourceKind != ItemContextSourceKind.InventorySlot)
		{
			ClearSharedContext();
			return;
		}

		int slotIndex = _contextInventorySlotIndex;
		ClearSharedContext();

		OnInventorySlotDropRequested(slotIndex);
	}

	private void OnSharedInfoPressed()
	{
		HideSharedContextMenu(clearContext: false);

		string title = "";
		string content = "";
		ItemContextSourceKind sourceKind = _contextSourceKind;
		int inventorySlotIndex = _contextInventorySlotIndex;
		EquipmentSlotId equipmentSlotId = _contextEquipmentSlotId;

		ClearSharedContext();

		if (sourceKind == ItemContextSourceKind.InventorySlot)
		{
			ItemEntry entry = _player?.Inventory?.GetEntryAt(inventorySlotIndex);
			if (entry == null || entry.IsEmpty() || entry.Item == null)
				return;

			title = entry.Item.DisplayName;
			content = BuildInventoryInfoText(entry.Item, entry.Amount);
		}
		else if (sourceKind == ItemContextSourceKind.EquipmentSlot)
		{
			EquippedItemViewData equipped = _player?.GetEquippedItemViewData(equipmentSlotId);
			if (equipped == null)
				return;

			title = equipped.DisplayName;
			content = BuildEquipmentInfoText(equipped);
		}
		else
		{
			return;
		}

		_sharedInfoDialog.Title = title;
		_sharedInfoContent.Text = content;
		_sharedInfoDialog.PopupCentered(new Vector2I(420, 260));
	}

	private void OnInventorySlotDropRequested(int slotIndex)
	{
		if (_player == null)
			return;

		PlayerInventoryState inventory = _player.Inventory;
		if (inventory == null)
			return;

		ItemEntry entry = inventory.GetEntryAt(slotIndex);
		if (entry == null || entry.IsEmpty() || entry.Item == null)
			return;

		int removedAmount = inventory.RemoveFromSlot(slotIndex, entry.Amount);
		if (removedAmount <= 0)
			return;

		int finalInventoryAmount = inventory.GetTotalAmount(entry.Item.Id);

		SpawnWorldPickupNearPlayer(entry.Item, removedAmount);
		InventoryItemDropped?.Invoke(entry.Item, removedAmount, finalInventoryAmount);
		Refresh();
	}

	private void SpawnWorldPickupNearPlayer(ItemData item, int amount)
	{
		if (_player == null || item == null || amount <= 0)
			return;

		if (_worldPickupScene == null)
			return;

		Node worldRoot = _player.GetParent();
		if (worldRoot == null)
			return;

		WorldPickup pickup = _worldPickupScene.Instantiate<WorldPickup>();
		if (pickup == null)
			return;

		pickup.Item = item;
		pickup.Amount = amount;

		worldRoot.AddChild(pickup);
		pickup.GlobalPosition = _player.GlobalPosition + new Vector2(42f, 20f);
	}

	private void RefreshStorageData()
	{
		if (_storageSubpanel == null)
			return;

		if (_baseRoot == null)
		{
			_storageSubpanel.SetEntries(Array.Empty<ItemEntry>());
			return;
		}

		_storageSubpanel.SetEntries(_baseRoot.GetStorageEntries());
	}

	private void RefreshSubView()
	{
		_storageSubpanel.Visible = _menuState.IsSubViewOpen && _menuState.CurrentSubAction == BaseActionId.Storage;
		_craftSubpanel.Visible = _menuState.IsSubViewOpen && _menuState.CurrentSubAction == BaseActionId.Craft;
		_healSubpanel.Visible = _menuState.IsSubViewOpen && _menuState.CurrentSubAction == BaseActionId.Heal;
		RefreshCraftAndHealPanelText();

		_subTitleLabel.Text = _menuState.CurrentSubAction switch
		{
			BaseActionId.Storage => "Хранилище",
			BaseActionId.Craft => "Станки",
			BaseActionId.Heal => "Лечение",
			_ => "Раздел"
		};
	}


	private string BuildInventoryInfoText(ItemData item, int amount)
	{
		if (item == null)
			return "Информация о предмете недоступна.";

		string totalWeightText = amount > 1
			? $"\nОбщий вес: {item.Weight * amount:0.##} кг"
			: string.Empty;

		string useText = item.CanUse()
			? $"\nЭффект: HP +{item.HealthRestore}, еда +{item.FoodRestore}, вода +{item.WaterRestore}, стамина +{item.StaminaRestore}"
			: string.Empty;

		string equipText = item.CanEquip()
			? $"\nЭкипировка: слот {BuildEquipmentSlotText(item.EquipSlotId)}, прочность {item.EquipmentMaxDurability}/{item.EquipmentMaxDurability}"
			: string.Empty;

		string hintText = string.IsNullOrWhiteSpace(item.UsageHint)
			? string.Empty
			: $"\nПодсказка: {item.UsageHint}";

		string description = string.IsNullOrWhiteSpace(item.Description)
			? "Описание пока не заполнено."
			: item.Description;

		return
			$"Название: {item.DisplayName}\n" +
			$"Id: {item.Id}\n" +
			$"Категория: {item.GetDisplayCategory()}\n" +
			$"Редкость: {BuildRarityText(item.Rarity)}\n" +
			$"Количество: {amount}\n" +
			$"Вес за единицу: {item.Weight:0.##} кг" +
			totalWeightText +
			$"\nМаксимум в стаке: {item.MaxStackSize}" +
			useText +
			equipText +
			hintText +
			$"\n\n{description}";
	}

	private string BuildEquipmentInfoText(EquippedItemViewData equipped)
	{
		if (equipped == null)
			return "Информация об экипировке недоступна.";

		ItemData item = equipped.SourceItem;
		string durabilityText = equipped.MaxDurability <= 0
			? "Прочность: неизвестно"
			: $"Прочность: {equipped.CurrentDurability}/{equipped.MaxDurability}";

		string sourceText = item == null
			? string.Empty
			: $"\nКатегория: {item.GetDisplayCategory()}\nРедкость: {BuildRarityText(item.Rarity)}\nВес: {item.Weight:0.##} кг";

		string description = item == null || string.IsNullOrWhiteSpace(item.Description)
			? "Описание пока не заполнено."
			: item.Description;

		return
			$"Название: {equipped.DisplayName}\n" +
			$"Слот: {BuildEquipmentSlotText(equipped.SlotId)}\n" +
			durabilityText +
			sourceText +
			$"\n\n{description}";
	}

	private static string BuildEquipmentSlotText(EquipmentSlotId slotId)
	{
		return slotId switch
		{
			EquipmentSlotId.Cape => "Плащ",
			EquipmentSlotId.Head => "Голова",
			EquipmentSlotId.Backpack => "Рюкзак",
			EquipmentSlotId.PrimaryWeapon => "Основное оружие",
			EquipmentSlotId.Chest => "Корпус",
			EquipmentSlotId.SecondaryWeapon => "Доп. оружие",
			EquipmentSlotId.Boots => "Ботинки",
			_ => slotId.ToString()
		};
	}

	private static string BuildRarityText(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => "Обычный",
			ItemRarity.Rare => "Редкий",
			ItemRarity.SuperRare => "Очень редкий",
			ItemRarity.Epic => "Эпический",
			ItemRarity.Legendary => "Легендарный",
			_ => rarity.ToString()
		};
	}

	private string BuildBaseInfoText(bool onBase)
	{
		int storageStacks = _baseRoot?.GetStorageEntries()?.Count ?? 0;
		string healthText = _player == null
			? "HP: ?"
			: $"HP: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth} | Еда: {_player.Needs.Food}/{_player.Needs.MaxFood} | Вода: {_player.Needs.Water}/{_player.Needs.MaxWater}";

		string upgrades = _baseRoot == null
			? ""
			: $" | База ур. {_baseRoot.Progress.BaseLevel}, склад {_baseRoot.Progress.GetUpgradeLevel("storage_box")}, верстак {_baseRoot.Progress.GetUpgradeLevel("workbench")}, медпункт {_baseRoot.Progress.GetUpgradeLevel("medical_station")}, генератор {_baseRoot.Progress.GetUpgradeLevel("generator")}, радио {_baseRoot.Progress.GetUpgradeLevel("radio_tower")}, стены {_baseRoot.Progress.GetUpgradeLevel("defensive_walls")}";

		return onBase
			? $"Игрок на базе. Действия доступны. {healthText}. В хранилище стаков: {storageStacks}.{upgrades}."
			: $"Игрок вне базы. Базовые действия заблокированы. {healthText}. Вернись в зелёную безопасную зону.";
	}

	private void BuildVisibleCraftAndHealPanels()
	{
		if (_craftSubpanel is VBoxContainer craftBox)
		{
			foreach (Node child in craftBox.GetChildren())
				child.QueueFree();

			_craftInfoLabel = new Label
			{
				Text = "Крафт и улучшения из ресурсов хранилища базы.",
				AutowrapMode = TextServer.AutowrapMode.WordSmart
			};
			_craftMedkitButton = new Button { Text = "Крафт: аптечка — 2 доски + 1 металл" };
			_craftPlateButton = new Button { Text = "Крафт: пластина — 2 камня + 2 металла" };
			_craftRepairKitButton = new Button { Text = "Крафт: ремонтный набор — нужен верстак" };
			_craftGeneratorPartButton = new Button { Text = "Крафт: деталь генератора — нужен верстак" };
			_craftRationButton = new Button { Text = "Крафт: рацион — нужен медпункт" };
			_craftWaterFilterButton = new Button { Text = "Крафт 0.3: фильтр воды — нужен верстак" };
			_craftToolKitButton = new Button { Text = "Крафт 0.3: набор инструментов — верстак ур.2" };
			_craftAmmoPackButton = new Button { Text = "Крафт 0.3: боеприпасы — нужен верстак" };
			_craftCampBeaconButton = new Button { Text = "Крафт 0.3: маяк лагеря — нужен генератор" };
			_upgradeStorageButton = new Button { Text = "Улучшить: хранилище" };
			_upgradeWorkbenchButton = new Button { Text = "Улучшить: верстак" };
			_upgradeMedbayButton = new Button { Text = "Улучшить: медпункт" };
			_upgradeGeneratorButton = new Button { Text = "Улучшить: генератор" };
			_upgradeRadioTowerButton = new Button { Text = "Улучшить 0.3: радиомачта" };
			_upgradeDefensiveWallsButton = new Button { Text = "Улучшить 0.3: стены базы" };
			craftBox.AddChild(_craftInfoLabel);
			craftBox.AddChild(_craftMedkitButton);
			craftBox.AddChild(_craftPlateButton);
			craftBox.AddChild(_craftRepairKitButton);
			craftBox.AddChild(_craftGeneratorPartButton);
			craftBox.AddChild(_craftRationButton);
			craftBox.AddChild(_craftWaterFilterButton);
			craftBox.AddChild(_craftToolKitButton);
			craftBox.AddChild(_craftAmmoPackButton);
			craftBox.AddChild(_craftCampBeaconButton);
			craftBox.AddChild(new HSeparator());
			craftBox.AddChild(_upgradeStorageButton);
			craftBox.AddChild(_upgradeWorkbenchButton);
			craftBox.AddChild(_upgradeMedbayButton);
			craftBox.AddChild(_upgradeGeneratorButton);
			craftBox.AddChild(_upgradeRadioTowerButton);
			craftBox.AddChild(_upgradeDefensiveWallsButton);
		}

		if (_healSubpanel is VBoxContainer healBox)
		{
			foreach (Node child in healBox.GetChildren())
				child.QueueFree();

			_healInfoLabel = new Label
			{
				Text = "Медпункт восстанавливает здоровье. Улучшение медпункта увеличивает лечение.",
				AutowrapMode = TextServer.AutowrapMode.WordSmart
			};
			_healNowButton = new Button { Text = "Полечиться" };
			healBox.AddChild(_healInfoLabel);
			healBox.AddChild(_healNowButton);
		}
	}

	private void RefreshCraftAndHealPanelText()
	{
		if (_craftInfoLabel != null)
		{
			_craftInfoLabel.Text =
				"Крафт уже подключён к хранилищу базы. Сначала собери ресурсы снаружи, вернись в безопасную зону, затем открой Станки.";
		}

		if (_healInfoLabel != null)
		{
			_healInfoLabel.Text = _player == null
				? "HP неизвестно."
				: $"Текущее здоровье: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth}. Сила лечения: {BaseFacilityEffectCalculator.GetHealingAmount(_baseRoot?.Progress?.GetUpgradeLevel("medical_station") ?? 0)} HP.";
		}

		bool canUseBaseActions = CanUseBaseActions();
		if (_craftMedkitButton != null) _craftMedkitButton.Disabled = !canUseBaseActions;
		if (_craftPlateButton != null) _craftPlateButton.Disabled = !canUseBaseActions;
		if (_craftRepairKitButton != null) _craftRepairKitButton.Disabled = !canUseBaseActions;
		if (_craftGeneratorPartButton != null) _craftGeneratorPartButton.Disabled = !canUseBaseActions;
		if (_craftRationButton != null) _craftRationButton.Disabled = !canUseBaseActions;
		if (_craftWaterFilterButton != null) _craftWaterFilterButton.Disabled = !canUseBaseActions;
		if (_craftToolKitButton != null) _craftToolKitButton.Disabled = !canUseBaseActions;
		if (_craftAmmoPackButton != null) _craftAmmoPackButton.Disabled = !canUseBaseActions;
		if (_craftCampBeaconButton != null) _craftCampBeaconButton.Disabled = !canUseBaseActions;
		if (_upgradeStorageButton != null) _upgradeStorageButton.Disabled = !canUseBaseActions;
		if (_upgradeWorkbenchButton != null) _upgradeWorkbenchButton.Disabled = !canUseBaseActions;
		if (_upgradeMedbayButton != null) _upgradeMedbayButton.Disabled = !canUseBaseActions;
		if (_upgradeGeneratorButton != null) _upgradeGeneratorButton.Disabled = !canUseBaseActions;
		if (_upgradeRadioTowerButton != null) _upgradeRadioTowerButton.Disabled = !canUseBaseActions;
		if (_upgradeDefensiveWallsButton != null) _upgradeDefensiveWallsButton.Disabled = !canUseBaseActions;
		if (_healNowButton != null) _healNowButton.Disabled = !canUseBaseActions || _player == null || _player.Stats.CurrentHealth >= _player.Stats.MaxHealth;
	}

	private void TryCraftById(string recipeId)
	{
		if (_baseRoot == null || _player == null || !CanUseBaseActions())
			return;

		foreach (CraftRecipe recipe in BasicCraftRecipeCatalog.CreateRecipes())
		{
			if (recipe.Id != recipeId)
				continue;

			CraftResult result = _baseRoot.TryCraft(recipe, _player.ItemCatalog);
			RefreshStorageData();
			RefreshCraftAndHealPanelText();

			if (_craftInfoLabel != null)
			{
				_craftInfoLabel.Text = result.Succeeded
					? $"Создано: {recipe.DisplayName}. Результат добавлен в хранилище."
					: $"Не удалось создать {recipe.DisplayName}: {BuildCraftFailText(result.Reason)}.";
			}

			return;
		}
	}

	private void TryUpgradeById(string upgradeId)
	{
		if (_baseRoot == null || _player == null || !CanUseBaseActions())
			return;

		foreach (BaseUpgradeDefinition upgrade in BasicBaseUpgradeCatalog.CreateUpgrades())
		{
			if (upgrade.Id != upgradeId)
				continue;

			bool upgraded = _baseRoot.TryUpgrade(upgrade);
			RefreshStorageData();
			RefreshCraftAndHealPanelText();
			_baseInfoLabel.Text = BuildBaseInfoText(_player.IsInsideBase);

			if (_craftInfoLabel != null)
			{
				_craftInfoLabel.Text = upgraded
					? $"Улучшение выполнено: {upgrade.DisplayName}."
					: $"Не удалось улучшить {upgrade.DisplayName}: не хватает ресурсов.";
			}

			return;
		}
	}

	private static string BuildCraftFailText(string reason)
	{
		return reason switch
		{
			"station_required" => "требуется станок или уровень базы",
			"storage_full" => "хранилище заполнено",
			"not_enough_resources" => "не хватает ресурсов",
			_ => "условия не выполнены"
		};
	}

	private void TryHealPlayerAtBase()
	{
		if (_player == null || !CanUseBaseActions())
			return;

		int healAmount = BaseFacilityEffectCalculator.GetHealingAmount(_baseRoot?.Progress?.GetUpgradeLevel("medical_station") ?? 0);
		bool healed = _player.HealAtBase(healAmount);
		RefreshCraftAndHealPanelText();
		_baseInfoLabel.Text = BuildBaseInfoText(_player.IsInsideBase);

		if (_healInfoLabel != null)
			_healInfoLabel.Text = healed
				? $"Лечение выполнено. HP: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth}."
				: $"Лечение не нужно. HP: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth}.";
	}

	private string BuildJournalText()
	{
		return
			"Цели версии 0.3.0:\n" +
			"1. Проверь полный цикл: добыча → база → хранилище → крафт → улучшение.\n" +
			"2. Новые рецепты 0.3: фильтр воды, набор инструментов, боеприпасы, маяк лагеря.\n" +
			"3. Новые улучшения 0.3: радиомачта и укреплённые стены базы.\n" +
			"4. Системы под капотом: ремонт экипировки, резервы склада, очередь крафта, открытие рецептов.\n" +
			"5. Выживание расширено: статус-эффекты, оценка риска, инструменты и биомы ресурсов.\n" +
			"6. Бой расширен: дальняя атака, боеприпасы и планировщик волн врагов.\n" +
			"7. Мир расширен: план тайлов карты и точки интереса по уровню опасности.\n" +
			"8. База расширена: энергия, оборона, рейды и потери ресурсов.\n" +
			"9. Сохранения расширены до save version 5: diff, backup policy, миграция старых данных.\n" +
			"10. Для проверки логики запусти тесты Version030BranchTests. Коммит: version 0.3.0: expand all development branches.";
	}}
