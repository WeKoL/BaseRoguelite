public static class InventoryMoveLogic
{
	public static InventoryMoveResult TryMoveOrMerge(InventoryState inventory, int fromSlot, int toSlot)
	{
		if (inventory == null || fromSlot < 0 || toSlot < 0 || fromSlot >= inventory.Slots.Count || toSlot >= inventory.Slots.Count) return InventoryMoveResult.Fail("invalid_slot");
		if (fromSlot == toSlot) return InventoryMoveResult.Fail("same_slot");
		InventorySlotState source = inventory.Slots[fromSlot];
		InventorySlotState target = inventory.Slots[toSlot];
		if (source == null || source.IsEmpty) return InventoryMoveResult.Fail("source_empty");
		if (target == null) return InventoryMoveResult.Fail("target_missing");
		if (target.IsEmpty)
		{
			target.SetItem(source.ItemId, source.Amount, source.MaxStackSize, source.WeightPerUnit);
			int moved = source.Amount; source.RemoveAmount(moved); return InventoryMoveResult.Success(moved);
		}
		if (target.ItemId == source.ItemId)
		{
			int moved = target.AddAmount(source.Amount); source.RemoveAmount(moved); return moved > 0 ? InventoryMoveResult.Success(moved) : InventoryMoveResult.Fail("target_full");
		}
		string itemId = source.ItemId; int amount = source.Amount; int maxStack = source.MaxStackSize; float weight = source.WeightPerUnit;
		source.SetItem(target.ItemId, target.Amount, target.MaxStackSize, target.WeightPerUnit);
		target.SetItem(itemId, amount, maxStack, weight);
		return InventoryMoveResult.Success(amount);
	}
}
