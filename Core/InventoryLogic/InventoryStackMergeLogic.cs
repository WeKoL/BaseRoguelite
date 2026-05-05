public static class InventoryStackMergeLogic
{
	public static int TryMergeIntoFirstMatchingStack(InventoryState inventory, int sourceSlotIndex)
	{
		if (inventory == null || sourceSlotIndex < 0 || sourceSlotIndex >= inventory.Slots.Count) return 0;
		InventorySlotState source = inventory.Slots[sourceSlotIndex]; if (source == null || source.IsEmpty) return 0;
		for (int i = 0; i < inventory.Slots.Count; i++) { if (i == sourceSlotIndex) continue; InventorySlotState target = inventory.Slots[i]; if (target == null || target.IsEmpty || target.ItemId != source.ItemId) continue; int moved = target.AddAmount(source.Amount); source.RemoveAmount(moved); return moved; }
		return 0;
	}
}
