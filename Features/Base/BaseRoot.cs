using Godot;
using System;
using System.Collections.Generic;

public partial class BaseRoot : Node2D
{
	private Area2D _safeZone;
	private CanvasItem _baseVisual;
	private BaseStorageState _storage = new();
	private readonly BaseProgressState _progress = new();

	public BaseStorageState Storage => _storage;
	public BaseProgressState Progress => _progress;

	public event Action<SafeZoneUnloadResult> InventoryUnloaded;

	public override void _Ready()
	{
		_safeZone = GetNode<Area2D>("SafeZone");
		_baseVisual = GetNodeOrNull<CanvasItem>("BaseVisual");

		if (_safeZone == null)
		{
			GD.PushError("BaseRoot: не найден узел SafeZone.");
			return;
		}

		_safeZone.BodyEntered += OnSafeZoneBodyEntered;
		_safeZone.BodyExited += OnSafeZoneBodyExited;
	}

	public IReadOnlyList<ItemEntry> GetStorageEntries()
	{
		return _storage.Entries;
	}

	public TransferResult TryMoveFromStorageToInventory(PlayerInventoryState inventory, string itemId, int amount)
	{
		if (inventory == null || string.IsNullOrWhiteSpace(itemId) || amount <= 0) return new TransferResult(amount,0);
		ItemData item = _storage.GetItemData(itemId);
		var definition = new InventoryItemDefinition(item.Id, item.MaxStackSize, item.Weight);
		return InventoryStorageTransferLogic.MoveToInventory(_storage.GetLogicState(), inventory.GetLogicState(), definition, amount);
	}

	public TransferResult TryMoveFromInventoryToStorage(PlayerInventoryState inventory, string itemId, int amount)
	{
		if (inventory == null || string.IsNullOrWhiteSpace(itemId) || amount <= 0) return new TransferResult(amount,0);
		ItemData item = inventory.GetItemData(itemId);
		var definition = new InventoryItemDefinition(item.Id, item.MaxStackSize, item.Weight);
		TransferResult result = InventoryStorageTransferLogic.MoveToStorage(inventory.GetLogicState(), _storage.GetLogicState(), definition, amount);
		if (result.MovedAmount > 0) _storage.RememberItem(item);
		return result;
	}

	public bool TryUpgrade(BaseUpgradeDefinition upgrade)
	{
		return BaseUpgradeLogic.TryApplyUpgrade(_storage.GetLogicState(), _progress, upgrade);
	}

	public CraftResult TryCraft(CraftRecipe recipe, ItemCatalog catalog)
	{
		if (recipe == null || catalog == null) return CraftResult.Fail("invalid_recipe");
		ItemData output = catalog.GetOrFallback(recipe.OutputItemId);
		_storage.RememberItem(output);
		var definition = new InventoryItemDefinition(output.Id, output.MaxStackSize, output.Weight);
		return CraftingLogic.TryCraft(_storage.GetLogicState(), recipe, definition, recipe.OutputAmount);
	}

	private void OnSafeZoneBodyEntered(Node2D body)
	{
		if (body is not PlayerController player)
			return;

		player.SetInsideBase(true);

		SafeZoneUnloadResult unloadResult = SafeZoneUnloadLogic.UnloadAll(
			player.Inventory.GetLogicState(),
			_storage.GetLogicState());

		foreach (SafeZoneUnloadEntry entry in unloadResult.Entries)
			_storage.RememberItem(player.Inventory.GetItemData(entry.ItemId));

		if (unloadResult.MovedAnything)
			InventoryUnloaded?.Invoke(unloadResult);

		if (_baseVisual != null)
			_baseVisual.Modulate = new Color(0.5f, 1.0f, 0.5f);
	}

	private void OnSafeZoneBodyExited(Node2D body)
	{
		if (body is not PlayerController player)
			return;

		player.SetInsideBase(false);

		if (_baseVisual != null)
			_baseVisual.Modulate = new Color(1.0f, 1.0f, 1.0f);
	}
}
