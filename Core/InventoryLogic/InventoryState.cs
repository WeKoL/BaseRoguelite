using System;
using System.Collections.Generic;

public sealed class InventoryState
{
	private readonly List<InventorySlotState> _slots = new();

	public IReadOnlyList<InventorySlotState> Slots => _slots;

	public int MaxSlots { get; }
	public float MaxCarryWeight { get; }

	public InventoryState(int maxSlots, float maxCarryWeight)
	{
		MaxSlots = Math.Max(1, maxSlots);
		MaxCarryWeight = Math.Max(1.0f, maxCarryWeight);

		for (int i = 0; i < MaxSlots; i++)
			_slots.Add(new InventorySlotState());
	}

	public float GetCurrentWeight()
	{
		float total = 0f;

		foreach (InventorySlotState slot in _slots)
		{
			if (slot.IsEmpty)
				continue;

			total += slot.Amount * slot.WeightPerUnit;
		}

		return total;
	}

	public float GetWeightFillRatio()
	{
		if (MaxCarryWeight <= 0f)
			return 0f;

		return Math.Clamp(GetCurrentWeight() / MaxCarryWeight, 0f, 1f);
	}

	public int GetTotalAmount(string itemId)
	{
		if (string.IsNullOrWhiteSpace(itemId))
			return 0;

		int total = 0;

		foreach (InventorySlotState slot in _slots)
		{
			if (slot.IsEmpty)
				continue;

			if (slot.ItemId == itemId)
				total += slot.Amount;
		}

		return total;
	}

	public int TryAddItem(InventoryItemDefinition item, int amount)
	{
		if (item == null || string.IsNullOrWhiteSpace(item.Id) || amount <= 0)
			return 0;

		int addedTotal = 0;
		int remaining = amount;

		remaining -= AddToExistingStacks(item, remaining, ref addedTotal);

		if (remaining > 0)
			remaining -= AddToEmptySlots(item, remaining, ref addedTotal);

		return addedTotal;
	}

	public int RemoveItem(string itemId, int amount)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
			return 0;

		int remaining = amount;
		int removed = 0;

		for (int i = _slots.Count - 1; i >= 0; i--)
		{
			InventorySlotState slot = _slots[i];

			if (slot.IsEmpty || slot.ItemId != itemId)
				continue;

			int take = Math.Min(slot.Amount, remaining);
			slot.RemoveAmount(take);

			removed += take;
			remaining -= take;

			if (remaining <= 0)
				break;
		}

		return removed;
	}

	public int RemoveFromSlot(int slotIndex, int amount)
	{
		if (slotIndex < 0 || slotIndex >= _slots.Count) return 0;
		if (amount <= 0) return 0;
		InventorySlotState slot = _slots[slotIndex];
		if (slot == null || slot.IsEmpty) return 0;
		int removed = Math.Min(slot.Amount, amount);
		slot.RemoveAmount(removed);
		return removed;
	}

	public bool TrySplitStack(int slotIndex, int amountToMove)
	{
		if (slotIndex < 0 || slotIndex >= _slots.Count || amountToMove <= 0) return false;
		InventorySlotState source = _slots[slotIndex];
		if (source == null || source.IsEmpty || source.Amount <= amountToMove) return false;
		InventorySlotState emptySlot = null;
		foreach (InventorySlotState slot in _slots) if (slot.IsEmpty) { emptySlot = slot; break; }
		if (emptySlot == null) return false;
		string itemId = source.ItemId; int maxStackSize = source.MaxStackSize; float weight = source.WeightPerUnit;
		source.RemoveAmount(amountToMove);
		emptySlot.SetItem(itemId, amountToMove, maxStackSize, weight);
		return true;
	}

	private int AddToExistingStacks(InventoryItemDefinition item, int amount, ref int addedTotal)
	{
		int remaining = amount;

		foreach (InventorySlotState slot in _slots)
		{
			if (slot.IsEmpty)
				continue;

			if (slot.ItemId != item.Id)
				continue;

			int maxByStack = slot.MaxStackSize - slot.Amount;
			if (maxByStack <= 0)
				continue;

			int maxByWeight = GetMaxAddableByWeight(item);
			if (maxByWeight <= 0)
				break;

			int toAdd = Math.Min(Math.Min(remaining, maxByStack), maxByWeight);
			if (toAdd <= 0)
				continue;

			slot.AddAmount(toAdd);
			addedTotal += toAdd;
			remaining -= toAdd;

			if (remaining <= 0)
				break;
		}

		return amount - remaining;
	}

	private int AddToEmptySlots(InventoryItemDefinition item, int amount, ref int addedTotal)
	{
		int remaining = amount;

		foreach (InventorySlotState slot in _slots)
		{
			if (!slot.IsEmpty)
				continue;

			int maxByWeight = GetMaxAddableByWeight(item);
			if (maxByWeight <= 0)
				break;

			int toAdd = Math.Min(Math.Min(remaining, item.MaxStackSize), maxByWeight);
			if (toAdd <= 0)
				continue;

			slot.SetItem(item.Id, toAdd, item.MaxStackSize, item.Weight);
			addedTotal += toAdd;
			remaining -= toAdd;

			if (remaining <= 0)
				break;
		}

		return amount - remaining;
	}

	private int GetMaxAddableByWeight(InventoryItemDefinition item)
	{
		if (item == null)
			return 0;

		if (item.Weight <= 0f)
			return int.MaxValue;

		float freeWeight = MaxCarryWeight - GetCurrentWeight();
		if (freeWeight <= 0f)
			return 0;

		return (int)MathF.Floor(freeWeight / item.Weight);
	}
}
