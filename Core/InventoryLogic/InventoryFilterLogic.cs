using System.Collections.Generic;
public static class InventoryFilterLogic
{
	public static IReadOnlyList<InventorySlotSnapshot> GetNonEmptySlots(InventoryState inventory)
	{
		List<InventorySlotSnapshot> result = new(); if (inventory == null) return result;
		for (int i = 0; i < inventory.Slots.Count; i++) { InventorySlotState slot = inventory.Slots[i]; if (slot == null || slot.IsEmpty) continue; result.Add(new InventorySlotSnapshot(i, slot.ItemId, slot.Amount, slot.MaxStackSize, slot.WeightPerUnit)); }
		return result;
	}
	public static int CountFreeSlots(InventoryState inventory)
	{
		if (inventory == null) return 0; int count = 0; foreach (InventorySlotState slot in inventory.Slots) if (slot == null || slot.IsEmpty) count++; return count;
	}
}
