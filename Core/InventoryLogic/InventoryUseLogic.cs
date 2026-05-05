public static class InventoryUseLogic
{
	public static bool TryUseConsumableFromSlot(InventoryState inventory, int slotIndex, ItemData item, PlayerStatsState stats)
	{
		if (inventory == null || item == null || stats == null || !item.CanUse()) return false;
		if (slotIndex < 0 || slotIndex >= inventory.Slots.Count) return false;
		InventorySlotState slot = inventory.Slots[slotIndex];
		if (slot == null || slot.IsEmpty || slot.ItemId != item.Id) return false;
		int healed = stats.Heal(item.HealthRestore);
		if (healed <= 0) return false;
		return inventory.RemoveFromSlot(slotIndex, 1) > 0;
	}
}
