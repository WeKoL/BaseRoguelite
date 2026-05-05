public static class InventoryStorageTransferLogic
{
	public static TransferResult MoveToStorage(InventoryState inventory, StorageState storage, InventoryItemDefinition item, int requestedAmount)
	{
		if (inventory == null || storage == null || item == null || requestedAmount <= 0) return new TransferResult(requestedAmount, 0);
		int toMove = System.Math.Min(inventory.GetTotalAmount(item.Id), requestedAmount);
		if (toMove <= 0) return new TransferResult(requestedAmount, 0);
		int removed = inventory.RemoveItem(item.Id, toMove);
		storage.AddItem(item.Id, removed, item.MaxStackSize);
		return new TransferResult(requestedAmount, removed);
	}
	public static TransferResult MoveAllToStorage(InventoryState inventory, StorageState storage)
	{
		if (inventory == null || storage == null) return new TransferResult(0,0);
		int movedTotal=0;
		for (int i=inventory.Slots.Count-1;i>=0;i--){ InventorySlotState slot=inventory.Slots[i]; if(slot.IsEmpty) continue; var item=new InventoryItemDefinition(slot.ItemId,slot.MaxStackSize,slot.WeightPerUnit); TransferResult result=MoveToStorage(inventory,storage,item,slot.Amount); movedTotal+=result.MovedAmount; }
		return new TransferResult(movedTotal,movedTotal);
	}
	public static TransferResult MoveToInventory(StorageState storage, InventoryState inventory, InventoryItemDefinition item, int requestedAmount)
	{
		if (inventory == null || storage == null || item == null || requestedAmount <= 0) return new TransferResult(requestedAmount,0);
		int toMove = System.Math.Min(storage.GetTotalAmount(item.Id), requestedAmount);
		if (toMove <= 0) return new TransferResult(requestedAmount,0);
		int added = inventory.TryAddItem(item, toMove);
		if (added <= 0) return new TransferResult(requestedAmount,0);
		storage.RemoveItem(item.Id, added);
		return new TransferResult(requestedAmount, added);
	}
}
