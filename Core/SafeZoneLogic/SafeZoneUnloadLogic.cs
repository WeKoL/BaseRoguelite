using System.Collections.Generic;

public static class SafeZoneUnloadLogic
{
	public static SafeZoneUnloadResult UnloadAll(InventoryState inventory, StorageState storage)
	{
		if (inventory == null || storage == null)
			return new SafeZoneUnloadResult(System.Array.Empty<SafeZoneUnloadEntry>());

		var itemOrder = new List<string>();
		var inventoryTotals = new Dictionary<string, int>();
		var storageTotalsBefore = new Dictionary<string, int>();

		foreach (InventorySlotState slot in inventory.Slots)
		{
			if (slot == null || slot.IsEmpty || string.IsNullOrWhiteSpace(slot.ItemId))
				continue;

			string itemId = slot.ItemId;

			if (!inventoryTotals.ContainsKey(itemId))
			{
				itemOrder.Add(itemId);
				inventoryTotals[itemId] = 0;
				storageTotalsBefore[itemId] = storage.GetTotalAmount(itemId);
			}

			inventoryTotals[itemId] += slot.Amount;
		}

		if (itemOrder.Count == 0)
			return new SafeZoneUnloadResult(System.Array.Empty<SafeZoneUnloadEntry>());

		InventoryStorageTransferLogic.MoveAllToStorage(inventory, storage);

		var entries = new List<SafeZoneUnloadEntry>(itemOrder.Count);

		foreach (string itemId in itemOrder)
		{
			int expectedAmount = inventoryTotals[itemId];
			int finalStorageAmount = storage.GetTotalAmount(itemId);
			int movedAmount = finalStorageAmount - storageTotalsBefore[itemId];

			if (movedAmount < 0)
				movedAmount = 0;

			if (movedAmount > expectedAmount)
				movedAmount = expectedAmount;

			entries.Add(new SafeZoneUnloadEntry(itemId, expectedAmount, movedAmount, finalStorageAmount));
		}

		return new SafeZoneUnloadResult(entries);
	}
}
