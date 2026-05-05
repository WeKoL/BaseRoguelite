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

	private string BuildBaseInfoText(bool onBase)
	{
		int storageStacks = _baseRoot?.GetStorageEntries()?.Count ?? 0;
		string healthText = _player == null
			? "HP: ?"
			: $"HP: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth}";

		return onBase
			? $"Игрок на базе. Действия доступны. {healthText}. В хранилище стаков: {storageStacks}."
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
				Text = "Крафт из ресурсов хранилища базы.",
				AutowrapMode = TextServer.AutowrapMode.WordSmart
			};
			_craftMedkitButton = new Button { Text = "Создать простую аптечку: 2 доски + 1 металл" };
			_craftPlateButton = new Button { Text = "Создать усиленную пластину: 2 камня + 2 металла" };
			craftBox.AddChild(_craftInfoLabel);
			craftBox.AddChild(_craftMedkitButton);
			craftBox.AddChild(_craftPlateButton);
		}

		if (_healSubpanel is VBoxContainer healBox)
		{
			foreach (Node child in healBox.GetChildren())
				child.QueueFree();

			_healInfoLabel = new Label
			{
				Text = "Медпункт восстанавливает здоровье, когда игрок на базе.",
				AutowrapMode = TextServer.AutowrapMode.WordSmart
			};
			_healNowButton = new Button { Text = "Полечиться: +25 HP" };
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
				: $"Текущее здоровье: {_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth}.";
		}

		bool canUseBaseActions = CanUseBaseActions();
		if (_craftMedkitButton != null) _craftMedkitButton.Disabled = !canUseBaseActions;
		if (_craftPlateButton != null) _craftPlateButton.Disabled = !canUseBaseActions;
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
					: $"Не удалось создать {recipe.DisplayName}: не хватает ресурсов.";
			}

			return;
		}
	}

	private void TryHealPlayerAtBase()
	{
		if (_player == null || !CanUseBaseActions())
			return;

		bool healed = _player.HealAtBase(25);
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
			"Тестовые цели версии 3.0 видимого патча:\n" +
			"1. Подойди к камню, доскам или металлолому и удерживай E — ресурс добывается в инвентарь.\n" +
			"2. Вернись в зелёную зону базы — ресурсы автоматически уйдут в хранилище.\n" +
			"3. На базе открой База → Станки и создай аптечку или пластину.\n" +
			"4. Подойди к красной зоне: сверху появится название опасной зоны.\n" +
			"5. ЛКМ рядом с врагом — тестовый удар ближнего боя.";
	}

	private static string BuildInventoryInfoText(ItemData item, int amount)
	{
		string description = string.IsNullOrWhiteSpace(item.Description)
			? "Без описания."
			: item.Description;

		string usageHint = string.IsNullOrWhiteSpace(item.UsageHint)
			? string.Empty
			: $"\nПодсказка: {item.UsageHint}";

		string equipText = item.CanEquip()
			? $"\nЭкипировка: да ({GetEquipSlotText(item.EquipSlotId)})\nПрочность: {item.EquipmentMaxDurability}"
			: "\nЭкипировка: нет";

		return
			$"Название: {item.DisplayName}\n" +
			$"Количество: {amount}\n" +
			$"Категория: {item.GetDisplayCategory()}\n" +
			$"Редкость: {GetRarityText(item.Rarity)}\n" +
			$"Вес одной единицы: {item.Weight:0.##}" +
			$"{equipText}\n\n" +
			$"{description}{usageHint}";
	}

	private static string BuildEquipmentInfoText(EquippedItemViewData equipped)
	{
		ItemData item = equipped.SourceItem;
		string category = item?.GetDisplayCategory() ?? "Предмет";
		string description = string.IsNullOrWhiteSpace(item?.Description)
			? "Без описания."
			: item.Description;

		string usageHint = string.IsNullOrWhiteSpace(item?.UsageHint)
			? string.Empty
			: $"\nПодсказка: {item.UsageHint}";

		return
			$"Название: {equipped.DisplayName}\n" +
			$"Слот: {GetEquipSlotText(equipped.SlotId)}\n" +
			$"Категория: {category}\n" +
			$"Прочность: {equipped.CurrentDurability}/{equipped.MaxDurability}\n" +
			$"Вес одной единицы: {item?.Weight:0.##}\n\n" +
			$"{description}{usageHint}";
	}

	private static string GetEquipSlotText(EquipmentSlotId slotId)
	{
		return slotId switch
		{
			EquipmentSlotId.Cape => "Плащ",
			EquipmentSlotId.Head => "Шлем",
			EquipmentSlotId.Backpack => "Рюкзак",
			EquipmentSlotId.PrimaryWeapon => "Оружие 1",
			EquipmentSlotId.Chest => "Броня",
			EquipmentSlotId.SecondaryWeapon => "Оружие 2",
			EquipmentSlotId.Boots => "Ботинки",
			_ => "Слот"
		};
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
}
