public static class ExpeditionExtractionLogic
{
	public static ExtractionResult TryExtractToBase(InventoryState inventory, StorageState storage, int fallbackMaxStackSize = 99)
	{
		if (inventory == null || storage == null) return ExtractionResult.Fail("invalid"); int saved = 0; for (int i = inventory.Slots.Count - 1; i >= 0; i--) { InventorySlotState slot = inventory.Slots[i]; if (slot == null || slot.IsEmpty) continue; int maxStack = slot.MaxStackSize > 0 ? slot.MaxStackSize : fallbackMaxStackSize; int added = storage.TryAddItem(slot.ItemId, slot.Amount, maxStack); if (added > 0) { inventory.RemoveFromSlot(i, added); saved += added; } } return ExtractionResult.Success(saved);
	}
}
