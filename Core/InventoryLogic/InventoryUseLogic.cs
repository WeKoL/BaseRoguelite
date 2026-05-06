public static class InventoryUseLogic
{
	public static bool TryUseConsumableFromSlot(InventoryState inventory, int slotIndex, ItemData item, PlayerStatsState stats)
	{
		return TryUseConsumableFromSlot(inventory, slotIndex, item, stats, null);
	}

	public static bool TryUseConsumableFromSlot(InventoryState inventory, int slotIndex, ItemData item, PlayerStatsState stats, SurvivalNeedsState needs)
	{
		if (inventory == null || item == null || !item.CanUse()) return false;
		if (slotIndex < 0 || slotIndex >= inventory.Slots.Count) return false;
		InventorySlotState slot = inventory.Slots[slotIndex];
		if (slot == null || slot.IsEmpty || slot.ItemId != item.Id) return false;

		int gained = 0;
		if (stats != null)
		{
			gained += stats.Heal(item.HealthRestore);
			gained += stats.RestoreStamina(item.StaminaRestore);
		}

		if (needs != null)
		{
			gained += needs.Eat(item.FoodRestore);
			gained += needs.Drink(item.WaterRestore);
		}

		if (gained <= 0) return false;
		return inventory.RemoveFromSlot(slotIndex, 1) > 0;
	}
}
