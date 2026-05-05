using Godot;
using System.Collections.Generic;

public partial class PlayerInventoryState : RefCounted
{
	private readonly InventoryState _inventory;
	private readonly Dictionary<string, ItemData> _itemLookup = new();

	public IReadOnlyList<InventorySlotState> Slots => _inventory.Slots;
	public int MaxSlots => _inventory.MaxSlots;
	public float MaxCarryWeight => _inventory.MaxCarryWeight;

	public PlayerInventoryState(int maxSlots, float maxCarryWeight)
	{
		_inventory = new InventoryState(maxSlots, maxCarryWeight);
	}

	public InventoryState GetLogicState()
	{
		return _inventory;
	}

	public float GetCurrentWeight()
	{
		return _inventory.GetCurrentWeight();
	}

	public float GetWeightFillRatio()
	{
		return _inventory.GetWeightFillRatio();
	}

	public int TryAddItem(ItemData item, int amount)
	{
		if (item == null || amount <= 0)
			return 0;

		RegisterItemData(item);

		var definition = new InventoryItemDefinition(
			item.Id,
			item.MaxStackSize,
			item.Weight
		);

		return _inventory.TryAddItem(definition, amount);
	}

	public PickupResult TryPickupItem(ItemData item, int amount)
	{
		if (item == null || amount <= 0)
			return new PickupResult(item?.Id, amount, 0, PickupFailureReason.InvalidRequest);

		RegisterItemData(item);

		var definition = new InventoryItemDefinition(
			item.Id,
			item.MaxStackSize,
			item.Weight
		);

		return InventoryPickupLogic.TryPickup(_inventory, definition, amount);
	}

	public int GetTotalAmount(string itemId)
	{
		return _inventory.GetTotalAmount(itemId);
	}

	public int RemoveItem(string itemId, int amount)
	{
		return _inventory.RemoveItem(itemId, amount);
	}

	public int RemoveFromSlot(int slotIndex, int amount)
	{
		return _inventory.RemoveFromSlot(slotIndex, amount);
	}

	public bool TrySplitStack(int slotIndex, int amountToMove)
	{
		return _inventory.TrySplitStack(slotIndex, amountToMove);
	}

	public void RegisterItemData(ItemData item)
	{
		if (item == null || string.IsNullOrWhiteSpace(item.Id))
			return;

		_itemLookup[item.Id] = item;
	}

	public ItemData GetItemData(string itemId)
	{
		if (string.IsNullOrWhiteSpace(itemId))
			return null;

		if (_itemLookup.TryGetValue(itemId, out ItemData item))
			return item;

		return CreateFallbackItem(itemId, 99, 1.0f);
	}

	public ItemEntry GetEntryAt(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= _inventory.Slots.Count)
			return null;

		InventorySlotState slot = _inventory.Slots[slotIndex];

		if (slot == null || slot.IsEmpty || string.IsNullOrWhiteSpace(slot.ItemId))
			return null;

		if (!_itemLookup.TryGetValue(slot.ItemId, out ItemData item))
			item = CreateFallbackItem(slot.ItemId, slot.MaxStackSize, slot.WeightPerUnit);

		return new ItemEntry(item, slot.Amount);
	}

	private ItemData CreateFallbackItem(string itemId, int maxStackSize, float weight)
	{
		return new ItemData
		{
			Id = itemId,
			DisplayName = itemId,
			Description = "",
			Category = ItemCategory.Material,
			Rarity = ItemRarity.Common,
			MaxStackSize = maxStackSize,
			Weight = weight <= 0f ? 1.0f : weight,
			UsageHint = "",
			Icon = null
		};
	}
}
