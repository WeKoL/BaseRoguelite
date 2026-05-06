using Godot;
using System.Collections.Generic;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	public float MoveSpeed { get; set; } = 180f;

	[Export]
	public float CameraMaxOffset { get; set; } = 140f;

	[Export]
	public float CameraDeadzoneRadiusPixels { get; set; } = 300f;

	[Export]
	public float CameraResponseExponent { get; set; } = 2.2f;

	[Export]
	public float Acceleration { get; set; } = 900f;

	[Export]
	public float Deceleration { get; set; } = 1100f;

	[Export]
	public float RotationLerpSpeed { get; set; } = 10f;

	[Export]
	public float CameraLookAheadDistance { get; set; } = 120f;

	[Export]
	public float CameraOffsetLerpSpeed { get; set; } = 7f;

	[Export]
	public Vector2 BaseZoom { get; set; } = new Vector2(2.2f, 2.2f);

	[Export]
	public Vector2 OutsideZoom { get; set; } = new Vector2(1.5f, 1.5f);

	[Export]
	public float ZoomDuration { get; set; } = 0.5f;

	[Export]
	public float InteractionHoldDuration { get; set; } = 0.6f;

	[Export]
	public float PickupRadius { get; set; } = 52f;

	public bool IsInsideBase { get; private set; }

	private Node2D _visual;
	private Camera2D _camera;
	private Tween _zoomTween;
	private InteractionProgressRing _interactionProgressRing;
	private GameMenu _gameMenu;

	private IMenuInteractable _currentInteractable;
	private readonly InteractionHoldState _interactionHoldState = new();
	private bool _ignoreInteractHoldUntilRelease;

	private PlayerInventoryState _inventory;
	private ItemCatalog _itemCatalog;
	private readonly PlayerStatsState _stats = new(100);
	private readonly SurvivalNeedsState _needs = new(100, 100);
	private WorldZoneState _currentZone;
	private readonly EquipmentState _equipment = new();
	private readonly Dictionary<string, ItemData> _equippedItemLookup = new();
	private float _meleeCooldown;
	private float _staminaRegenBuffer;
	private float _combatMessageTimer;
	private string _lastCombatMessage = "ЛКМ — ударить ближайшего врага";
	private float _needsTickBuffer;
	private float _sprintSpendBuffer;
	private bool _isSprinting;

	public PlayerInventoryState Inventory => _inventory;
	public ItemCatalog ItemCatalog => _itemCatalog;
	public PlayerStatsState Stats => _stats;
	public SurvivalNeedsState Needs => _needs;
	public WorldZoneState CurrentZone => _currentZone;
	public EquipmentState Equipment => _equipment;
	
	public event Action<ItemData, int, int> ItemPickedUp;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Pausable;
		AddToGroup("player");

		_visual = GetNode<Node2D>("Visual");
		_camera = GetNode<Camera2D>("Camera2D");
		_camera.Zoom = OutsideZoom;

		_interactionProgressRing = GetNodeOrNull<InteractionProgressRing>("InteractionProgressRing");
		if (_interactionProgressRing != null)
			_interactionProgressRing.SetProgressState(false, 0f);

		_inventory = new PlayerInventoryState(16, 60.0f);
		_itemCatalog = ItemCatalog.LoadFromFolder();
		BuildDebugInventoryData();
		BuildDebugEquipmentData();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (GetTree().Paused)
			return;

		if (_gameMenu != null && _gameMenu.IsOpen)
			return;

		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
			{
				TryMeleeAttackNearestEnemy();
				GetViewport().SetInputAsHandled();
			}

			return;
		}

		if (@event is not InputEventKey keyEvent)
			return;

		if (!keyEvent.Pressed || keyEvent.Echo)
			return;

		if (@event.IsActionPressed("quick_use"))
		{
			if (TryUseBestConsumable())
				GetViewport().SetInputAsHandled();
			return;
		}

		if (!@event.IsActionPressed("interact"))
			return;

		if (TryPickupNearest())
		{
			_ignoreInteractHoldUntilRelease = true;
			ResetInteractionProgress();
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused)
		{
			Velocity = Vector2.Zero;
			return;
		}

		float dt = (float)delta;

		UpdateMovement(dt);
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (GetTree().Paused)
		{
			ResetInteractionProgress();
			return;
		}

		float dt = (float)delta;

		UpdateClosestInteractable();
		UpdateInteractionHold(dt);
		UpdateVisualRotation(dt);
		UpdateCameraOffset(dt);
		UpdateCombatTimers(dt);
		UpdateSurvivalNeeds(dt);
	}

	public void BindGameMenu(GameMenu gameMenu)
	{
		_gameMenu = gameMenu;
	}

	public void SetInsideBase(bool value)
	{
		if (IsInsideBase == value)
			return;

		IsInsideBase = value;
		AnimateZoom(IsInsideBase ? BaseZoom : OutsideZoom);
	}

	public string GetInteractionPrompt()
	{
		return _currentInteractable?.GetInteractionText(this) ?? string.Empty;
	}

	public string BuildGameplayStatusText()
	{
		string zoneText = _currentZone == null
			? "Зона: обычная"
			: $"Зона: {_currentZone.DisplayName} | опасность {_currentZone.DangerLevel}";

		string baseText = IsInsideBase ? "База: внутри, меню базы доступно" : "База: вне базы";
		string needsText = $"Еда: {_needs.Food}/{_needs.MaxFood} | Вода: {_needs.Water}/{_needs.MaxWater}";
		string interactionText = GetInteractionPrompt();
		if (string.IsNullOrWhiteSpace(interactionText))
			interactionText = "E — подобрать ближайший предмет / удерживать E у объекта";

		return
			$"HP: {_stats.CurrentHealth}/{_stats.MaxHealth} | Стамина: {_stats.CurrentStamina}/{_stats.MaxStamina} | {needsText}\n" +
			$"{zoneText} | {baseText} | Бег: {(_isSprinting ? "да" : "нет")}\n" +
			$"{interactionText}\n" +
			$"{_lastCombatMessage}";
	}

	public void SetCurrentZone(WorldZoneState zone)
	{
		_currentZone = zone;
	}

	private void UpdateMovement(float delta)
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		bool wantsSprint = Input.IsActionPressed("sprint") && inputDirection != Vector2.Zero && _stats.CurrentStamina > 0;
		_isSprinting = wantsSprint;
		if (_isSprinting)
		{
			_sprintSpendBuffer += delta * 18f;
			int staminaCost = Mathf.FloorToInt(_sprintSpendBuffer);
			if (staminaCost > 0)
			{
				_isSprinting = _stats.TrySpendStamina(staminaCost);
				_sprintSpendBuffer -= staminaCost;
			}
		}
		else
		{
			_sprintSpendBuffer = 0f;
		}

		float speedMultiplier = _isSprinting ? 1.45f : 1.0f;
		Vector2 targetVelocity = inputDirection * MoveSpeed * speedMultiplier;

		float speed = inputDirection == Vector2.Zero ? Deceleration : Acceleration;
		Velocity = Velocity.MoveToward(targetVelocity, speed * delta);
	}

	private void UpdateVisualRotation(float delta)
	{
		if (_visual == null)
			return;

		Vector2 toMouse = GetGlobalMousePosition() - GlobalPosition;
		if (toMouse.LengthSquared() < 0.001f)
			return;

		float targetRotation = toMouse.Angle() + Mathf.Pi / 2f;
		_visual.Rotation = Mathf.LerpAngle(_visual.Rotation, targetRotation, RotationLerpSpeed * delta);
	}

	private void UpdateCameraOffset(float delta)
	{
		if (_camera == null)
			return;

		Vector2 viewportSize = GetViewportRect().Size;
		Vector2 screenCenter = viewportSize * 0.5f;
		Vector2 mouseScreenPos = GetViewport().GetMousePosition();

		Vector2 fromCenter = mouseScreenPos - screenCenter;
		float distance = fromCenter.Length();

		float maxInputRadius = Mathf.Min(viewportSize.X, viewportSize.Y) * 0.5f;
		float deadzone = Mathf.Min(CameraDeadzoneRadiusPixels, maxInputRadius - 1f);

		Vector2 targetOffset = Vector2.Zero;

		if (distance > deadzone)
		{
			float usableRadius = maxInputRadius - deadzone;
			float normalized = (distance - deadzone) / usableRadius;
			normalized = Mathf.Clamp(normalized, 0f, 1f);

			float curved = Mathf.Pow(normalized, CameraResponseExponent);
			Vector2 direction = fromCenter.Normalized();

			targetOffset = direction * (CameraMaxOffset * curved);
		}

		_camera.Position = _camera.Position.Lerp(targetOffset, CameraOffsetLerpSpeed * delta);
	}

	private void UpdateClosestInteractable()
	{
		if (!IsInteractableAlive(_currentInteractable))
			_currentInteractable = null;

		var candidates = new List<InteractionCandidate>();

		foreach (Node node in GetTree().GetNodesInGroup("menu_interactable"))
		{
			if (!GodotObject.IsInstanceValid(node))
				continue;

			if (node is not IMenuInteractable interactable)
				continue;

			if (!IsInteractableAlive(interactable))
				continue;

			if (node is not Node2D node2D)
				continue;

			Vector2 center = node2D.GlobalPosition;

			candidates.Add(new InteractionCandidate(
				interactable,
				center.X,
				center.Y,
				interactable.GetInteractionRadius()
			));
		}

		object bestTarget = InteractionSelectionLogic.SelectClosest(
			GlobalPosition.X,
			GlobalPosition.Y,
			candidates
		);

		IMenuInteractable bestInteractable = bestTarget as IMenuInteractable;

		if (ReferenceEquals(_currentInteractable, bestInteractable))
			return;

		TrySetInteractableSelected(_currentInteractable, false);

		_currentInteractable = bestInteractable;

		TrySetInteractableSelected(_currentInteractable, true);

		ResetInteractionProgress();
	}

	private void UpdateInteractionHold(float delta)
	{
		if (_ignoreInteractHoldUntilRelease)
		{
			if (!Input.IsActionPressed("interact"))
				_ignoreInteractHoldUntilRelease = false;

			ResetInteractionProgress();
			return;
		}

		if (!IsInteractableAlive(_currentInteractable))
			_currentInteractable = null;

		bool hasTarget = _currentInteractable != null;
		bool isPressed = Input.IsActionPressed("interact");

		InteractionHoldUpdateResult result = _interactionHoldState.Update(
			hasTarget,
			isPressed,
			delta,
			InteractionHoldDuration
		);

		_interactionProgressRing?.SetProgressState(result.IsActive, result.Progress);

		if (result.JustCompleted && _currentInteractable != null)
		{
			if (!IsInteractableAlive(_currentInteractable))
			{
				_currentInteractable = null;
				_interactionProgressRing?.SetProgressState(false, 0f);
				return;
			}

			IMenuInteractable interactable = _currentInteractable;
			TrySetInteractableSelected(interactable, false);
			_currentInteractable = null;
			interactable.Interact(this, _gameMenu);
			_interactionProgressRing?.SetProgressState(false, 0f);
		}
	}

	private bool TryPickupNearest()
	{
		WorldPickup pickup = FindClosestPickup();
		if (pickup == null || !pickup.IsAvailable || pickup.Item == null || pickup.Amount <= 0)
			return false;

		ItemData item = pickup.Item;
		int amount = pickup.Amount;

		bool pickedUp = pickup.TryPickup(Inventory);
		if (!pickedUp)
			return false;

		int finalInventoryAmount = Inventory.GetTotalAmount(item.Id);
		ItemPickedUp?.Invoke(item, amount, finalInventoryAmount);

		return true;
	}

	private WorldPickup FindClosestPickup()
	{
		WorldPickup closest = null;
		float bestDistanceSq = PickupRadius * PickupRadius;

		foreach (Node node in GetTree().GetNodesInGroup("world_pickups"))
		{
			if (!GodotObject.IsInstanceValid(node))
				continue;

			if (node is not WorldPickup pickup)
				continue;

			if (!pickup.IsAvailable)
				continue;

			float distanceSq = GlobalPosition.DistanceSquaredTo(pickup.GlobalPosition);
			if (distanceSq > bestDistanceSq)
				continue;

			bestDistanceSq = distanceSq;
			closest = pickup;
		}

		return closest;
	}

	private static void TrySetInteractableSelected(IMenuInteractable interactable, bool isSelected)
	{
		if (!IsInteractableAlive(interactable))
			return;

		try
		{
			interactable.SetSelectedByPlayer(isSelected);
		}
		catch (System.ObjectDisposedException)
		{
			// Object was destroyed between alive-check and selection update.
		}
	}

	private static bool IsInteractableAlive(IMenuInteractable interactable)
	{
		if (interactable == null)
			return false;

		if (interactable is not GodotObject godotObject)
			return true;

		if (!GodotObject.IsInstanceValid(godotObject))
			return false;

		if (godotObject is Node node)
		{
			if (node.IsQueuedForDeletion())
				return false;

			if (!node.IsInsideTree())
				return false;
		}

		return true;
	}


	private void UpdateCombatTimers(float delta)
	{
		_meleeCooldown = Mathf.Max(0f, _meleeCooldown - delta);

		if (!_isSprinting)
		{
			_staminaRegenBuffer += delta * 14f;
			int staminaToRestore = Mathf.FloorToInt(_staminaRegenBuffer);
			if (staminaToRestore > 0)
			{
				_stats.RestoreStamina(staminaToRestore);
				_staminaRegenBuffer -= staminaToRestore;
			}
		}
		else
		{
			_staminaRegenBuffer = 0f;
		}

		if (_combatMessageTimer > 0f)
		{
			_combatMessageTimer = Mathf.Max(0f, _combatMessageTimer - delta);
			if (_combatMessageTimer <= 0f)
				_lastCombatMessage = "ЛКМ — ударить ближайшего врага";
		}
	}

	private bool TryMeleeAttackNearestEnemy()
	{
		if (_meleeCooldown > 0f)
		{
			SetCombatMessage("Удар ещё на перезарядке", 0.5f);
			return false;
		}

		const int staminaCost = 12;
		if (!_stats.TrySpendStamina(staminaCost))
		{
			SetCombatMessage("Не хватает стамины для удара", 0.9f);
			return false;
		}

		SimpleEnemy enemy = FindClosestEnemy(78f);
		_meleeCooldown = 0.55f;

		if (enemy == null)
		{
			SetCombatMessage("Удар в воздух: враг слишком далеко", 0.8f);
			return false;
		}

		int damage = 15;
		int actualDamage = enemy.TakeDamage(damage);
		SetCombatMessage(actualDamage > 0
			? $"Попадание по врагу: -{actualDamage} HP"
			: "Враг уже повержен", 1.0f);

		return actualDamage > 0;
	}

	private SimpleEnemy FindClosestEnemy(float radius)
	{
		SimpleEnemy closest = null;
		float bestDistanceSq = radius * radius;

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (!GodotObject.IsInstanceValid(node) || node is not SimpleEnemy enemy || !enemy.IsAlive)
				continue;

			float distanceSq = GlobalPosition.DistanceSquaredTo(enemy.GlobalPosition);
			if (distanceSq > bestDistanceSq)
				continue;

			bestDistanceSq = distanceSq;
			closest = enemy;
		}

		return closest;
	}

	private void SetCombatMessage(string message, float duration)
	{
		_lastCombatMessage = string.IsNullOrWhiteSpace(message) ? "ЛКМ — ударить ближайшего врага" : message;
		_combatMessageTimer = Mathf.Max(0.1f, duration);
	}

	private void UpdateSurvivalNeeds(float delta)
	{
		_needsTickBuffer += delta;
		if (_needsTickBuffer < 8f)
			return;

		SurvivalNeedsUpdateResult result = SurvivalNeedsLogic.Tick(_needs, _stats, _needsTickBuffer, IsInsideBase);
		_needsTickBuffer = 0f;

		if (result.IsCritical)
			SetCombatMessage(result.HealthLost > 0 ? $"Голод/жажда наносят урон: {result.HealthLost}" : "Критический голод или жажда", 1.4f);
		else if (_needs.IsHungry || _needs.IsThirsty)
			SetCombatMessage("Нужна еда или вода. H — быстрый расходник", 1.4f);
	}

	private void ResetInteractionProgress()
	{
		_interactionHoldState.Reset();
		_interactionProgressRing?.SetProgressState(false, 0f);
	}

	private void AnimateZoom(Vector2 targetZoom)
	{
		if (_camera == null)
			return;

		if (_zoomTween != null && _zoomTween.IsValid())
			_zoomTween.Kill();

		_zoomTween = CreateTween();
		_zoomTween.TweenProperty(_camera, "zoom", targetZoom, ZoomDuration);
	}
	public EquippedItemViewData GetEquippedItemViewData(EquipmentSlotId slotId)
	{
		EquippedItemState equipped = _equipment.GetEquippedItem(slotId);
		if (equipped == null)
			return null;

		ItemData itemData = ResolveEquipmentItemData(equipped.ItemId);

		return new EquippedItemViewData(
			slotId,
			equipped.DisplayName,
			itemData,
			itemData?.Icon,
			equipped.CurrentDurability,
			equipped.MaxDurability);
	}
	public bool TryUseInventorySlot(int slotIndex)
	{
		ItemEntry entry = _inventory?.GetEntryAt(slotIndex);
		if (entry == null || entry.IsEmpty() || entry.Item == null) return false;
		bool used = InventoryUseLogic.TryUseConsumableFromSlot(_inventory.GetLogicState(), slotIndex, entry.Item, _stats, _needs);
		if (used) SetCombatMessage($"Использовано: {entry.Item.DisplayName}", 1.2f);
		return used;
	}

	public bool TryUseBestConsumable()
	{
		if (_inventory == null) return false;
		for (int i = 0; i < _inventory.MaxSlots; i++)
		{
			ItemEntry entry = _inventory.GetEntryAt(i);
			if (entry == null || entry.IsEmpty() || entry.Item == null || !entry.Item.CanUse()) continue;

			bool wantsHealth = entry.Item.HealthRestore > 0 && _stats.CurrentHealth < _stats.MaxHealth;
			bool wantsFood = entry.Item.FoodRestore > 0 && _needs.Food < _needs.MaxFood;
			bool wantsWater = entry.Item.WaterRestore > 0 && _needs.Water < _needs.MaxWater;
			bool wantsStamina = entry.Item.StaminaRestore > 0 && _stats.CurrentStamina < _stats.MaxStamina;
			if (!wantsHealth && !wantsFood && !wantsWater && !wantsStamina) continue;

			return TryUseInventorySlot(i);
		}

		SetCombatMessage("Нет подходящего расходника для H", 1.1f);
		return false;
	}

	public int ApplyDamage(int damage)
	{
		int actual = _stats.TakeDamage(damage);
		if (actual > 0)
			SetCombatMessage($"Получен урон: {actual}. HP: {_stats.CurrentHealth}/{_stats.MaxHealth}", 1.4f);
		return actual;
	}

	public bool HealAtBase(int amount)
	{
		int healed = _stats.Heal(amount);
		if (healed <= 0)
			return false;

		SetCombatMessage($"Лечение: +{healed} HP", 1.6f);
		return true;
	}

	public bool TryEquipFromInventorySlot(int slotIndex)
	{
		ItemEntry entry = _inventory?.GetEntryAt(slotIndex);
		if (entry == null || entry.IsEmpty() || entry.Item == null)
			return false;

		ItemData item = entry.Item;
		if (!item.CanEquip())
			return false;

		EquippedItemState currentEquipped = _equipment.GetEquippedItem(item.EquipSlotId);
		ItemData currentEquippedItemData = null;

		if (currentEquipped != null)
		{
			currentEquippedItemData = ResolveEquipmentItemData(currentEquipped.ItemId)
				?? BuildFallbackEquipmentItemData(currentEquipped);
		}

		int removed = _inventory.RemoveFromSlot(slotIndex, 1);
		if (removed <= 0)
			return false;

		if (currentEquippedItemData != null)
		{
			int returnedToInventory = _inventory.TryAddItem(currentEquippedItemData, 1);
			if (returnedToInventory <= 0)
			{
				_inventory.TryAddItem(item, 1);
				return false;
			}
		}

		EquipmentItemDefinition definition = new EquipmentItemDefinition(
			item.Id,
			item.EquipSlotId,
			item.EquipmentMaxDurability,
			item.DisplayName);

		EquipResult result = _equipment.EquipToSlot(item.EquipSlotId, definition);
		if (!result.Succeeded)
		{
			if (currentEquippedItemData != null)
				_inventory.RemoveItem(currentEquippedItemData.Id, 1);

			_inventory.TryAddItem(item, 1);
			return false;
		}

		_equippedItemLookup[item.Id] = item;

		if (currentEquippedItemData != null)
			_equippedItemLookup[currentEquippedItemData.Id] = currentEquippedItemData;

		return true;
	}

	public bool TryUnequip(EquipmentSlotId slotId)
	{
		EquippedItemState equipped = _equipment.GetEquippedItem(slotId);
		if (equipped == null)
			return false;

		ItemData itemData = ResolveEquipmentItemData(equipped.ItemId)
			?? BuildFallbackEquipmentItemData(equipped);

		_inventory.RegisterItemData(itemData);

		InventoryItemDefinition inventoryDefinition = new InventoryItemDefinition(
			itemData.Id,
			1,
			itemData.Weight);

		UnequipToInventoryResult result = EquipmentInventoryTransferLogic.TryUnequipToInventory(
			_equipment,
			_inventory.GetLogicState(),
			slotId,
			inventoryDefinition);

		if (!result.Succeeded)
			return false;

		_equippedItemLookup[itemData.Id] = itemData;
		return true;
	}

	private ItemData ResolveEquipmentItemData(string itemId)
	{
		if (string.IsNullOrWhiteSpace(itemId))
			return null;

		if (_equippedItemLookup.TryGetValue(itemId, out ItemData equippedData))
			return equippedData;

		return _inventory?.GetItemData(itemId);
	}

	private ItemData BuildFallbackEquipmentItemData(EquippedItemState equipped)
	{
		ItemCategory category =
			equipped.SlotId == EquipmentSlotId.PrimaryWeapon || equipped.SlotId == EquipmentSlotId.SecondaryWeapon
				? ItemCategory.Weapon
				: ItemCategory.Product;

		return new ItemData
		{
			Id = equipped.ItemId,
			DisplayName = equipped.BaseDisplayName,
			Description = string.Empty,
			Category = category,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 1.0f,
			UsageHint = string.Empty,
			Icon = null,
			IsEquippable = true,
			EquipSlotId = equipped.SlotId,
			EquipmentMaxDurability = equipped.MaxDurability
		};
	}

	private void BuildDebugEquipmentData()
	{
		ItemData cloak = new ItemData
		{
			Id = "patched_cloak",
			DisplayName = "Потрёпанный плащ",
			Description = "Старый плащ с усиленными вставками.",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 2.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Cape,
			EquipmentMaxDurability = 80
		};

		ItemData helmet = new ItemData
		{
			Id = "scrap_helmet",
			DisplayName = "Шлем из лома",
			Description = "Грубый, но полезный шлем.",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 3.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Head,
			EquipmentMaxDurability = 100
		};

		ItemData backpack = new ItemData
		{
			Id = "field_pack",
			DisplayName = "Полевой рюкзак",
			Description = "Рюкзак для вылазок.",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 2.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Backpack,
			EquipmentMaxDurability = 90
		};

		ItemData rifle = new ItemData
		{
			Id = "pipe_rifle",
			DisplayName = "Самодельная винтовка",
			Description = "Собрана вручную из подручных деталей.",
			Category = ItemCategory.Weapon,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 5.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.PrimaryWeapon,
			EquipmentMaxDurability = 100
		};

		ItemData armor = new ItemData
		{
			Id = "scrap_armor",
			DisplayName = "Бронежилет из пластин",
			Description = "Тяжёлый импровизированный доспех.",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 6.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Chest,
			EquipmentMaxDurability = 120
		};

		ItemData pistol = new ItemData
		{
			Id = "rusty_pistol",
			DisplayName = "Ржавый пистолет",
			Description = "Старый пистолет как запасное оружие.",
			Category = ItemCategory.Weapon,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 2.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.SecondaryWeapon,
			EquipmentMaxDurability = 70
		};

		ItemData boots = new ItemData
		{
			Id = "work_boots",
			DisplayName = "Рабочие ботинки",
			Description = "Надёжные ботинки для грубой местности.",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Common,
			MaxStackSize = 1,
			Weight = 2.0f,
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Boots,
			EquipmentMaxDurability = 90
		};

		ItemData reinforcedHelmet = new ItemData
		{
			Id = "reinforced_helmet",
			DisplayName = "Усиленный шлем",
			Description = "Более крепкий шлем для проверки действия «Надеть».",
			Category = ItemCategory.Product,
			Rarity = ItemRarity.Rare,
			MaxStackSize = 1,
			Weight = 4.0f,
			UsageHint = "ПКМ по предмету в инвентаре → Надеть.",
			IsEquippable = true,
			EquipSlotId = EquipmentSlotId.Head,
			EquipmentMaxDurability = 140
		};

		_equippedItemLookup[cloak.Id] = cloak;
		_equippedItemLookup[helmet.Id] = helmet;
		_equippedItemLookup[backpack.Id] = backpack;
		_equippedItemLookup[rifle.Id] = rifle;
		_equippedItemLookup[armor.Id] = armor;
		_equippedItemLookup[pistol.Id] = pistol;
		_equippedItemLookup[boots.Id] = boots;

		_inventory.TryAddItem(reinforcedHelmet, 1);

		_equipment.EquipToSlot(
			EquipmentSlotId.Cape,
			new EquipmentItemDefinition(cloak.Id, EquipmentSlotId.Cape, 80, cloak.DisplayName),
			EquipmentModifierId.Worn);

		_equipment.EquipToSlot(
			EquipmentSlotId.Head,
			new EquipmentItemDefinition(helmet.Id, EquipmentSlotId.Head, 100, helmet.DisplayName),
			EquipmentModifierId.None);

		_equipment.EquipToSlot(
			EquipmentSlotId.Backpack,
			new EquipmentItemDefinition(backpack.Id, EquipmentSlotId.Backpack, 90, backpack.DisplayName),
			EquipmentModifierId.Reinforced);

		_equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			new EquipmentItemDefinition(rifle.Id, EquipmentSlotId.PrimaryWeapon, 100, rifle.DisplayName),
			EquipmentModifierId.Calibrated);

		_equipment.EquipToSlot(
			EquipmentSlotId.Chest,
			new EquipmentItemDefinition(armor.Id, EquipmentSlotId.Chest, 120, armor.DisplayName),
			EquipmentModifierId.Reinforced);

		_equipment.EquipToSlot(
			EquipmentSlotId.SecondaryWeapon,
			new EquipmentItemDefinition(pistol.Id, EquipmentSlotId.SecondaryWeapon, 70, pistol.DisplayName),
			EquipmentModifierId.Worn);

		_equipment.EquipToSlot(
			EquipmentSlotId.Boots,
			new EquipmentItemDefinition(boots.Id, EquipmentSlotId.Boots, 90, boots.DisplayName),
			EquipmentModifierId.None);

		_equipment.ReduceDurability(EquipmentSlotId.Cape, 12);
		_equipment.ReduceDurability(EquipmentSlotId.PrimaryWeapon, 18);
		_equipment.ReduceDurability(EquipmentSlotId.SecondaryWeapon, 25);
		_equipment.ReduceDurability(EquipmentSlotId.Boots, 8);
	}

	private void BuildDebugInventoryData()
	{
		DebugStartingLoadout.FillInventory(_inventory, _itemCatalog);
	}
}
