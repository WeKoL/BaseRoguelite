public static class EquipmentInventoryTransferLogic
{
	public static UnequipToInventoryResult TryUnequipToInventory(
		EquipmentState equipment,
		InventoryState inventory,
		EquipmentSlotId slotId,
		InventoryItemDefinition inventoryItem)
	{
		if (equipment == null || inventory == null || inventoryItem == null || !inventoryItem.IsValid())
			return UnequipToInventoryResult.Fail(UnequipToInventoryFailureReason.InvalidRequest);

		EquippedItemState equippedItem = equipment.GetEquippedItem(slotId);
		if (equippedItem == null)
			return UnequipToInventoryResult.Fail(UnequipToInventoryFailureReason.NoEquippedItem);

		if (equippedItem.ItemId != inventoryItem.Id)
			return UnequipToInventoryResult.Fail(UnequipToInventoryFailureReason.ItemMismatch);

		int added = inventory.TryAddItem(inventoryItem, 1);
		if (added <= 0)
			return UnequipToInventoryResult.Fail(UnequipToInventoryFailureReason.InventoryFull);

		EquippedItemState removed = equipment.Unequip(slotId);
		if (removed == null)
		{
			inventory.RemoveItem(inventoryItem.Id, 1);
			return UnequipToInventoryResult.Fail(UnequipToInventoryFailureReason.NoEquippedItem);
		}

		return UnequipToInventoryResult.Success(removed);
	}
}
