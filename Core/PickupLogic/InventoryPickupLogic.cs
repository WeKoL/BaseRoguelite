using System;

public static class InventoryPickupLogic
{
	public static PickupResult TryPickup(
		InventoryState inventory,
		InventoryItemDefinition item,
		int amount)
	{
		if (inventory == null || item == null || string.IsNullOrWhiteSpace(item.Id) || amount <= 0)
			return new PickupResult(item?.Id, amount, 0, PickupFailureReason.InvalidRequest);

		int slotCapacity = CalculateSlotCapacity(inventory, item);
		if (slotCapacity < amount)
			return new PickupResult(item.Id, amount, 0, PickupFailureReason.NotEnoughSlots);

		int weightCapacity = CalculateWeightCapacity(inventory, item);
		if (weightCapacity < amount)
			return new PickupResult(item.Id, amount, 0, PickupFailureReason.NotEnoughWeightCapacity);

		int pickedAmount = inventory.TryAddItem(item, amount);
		PickupFailureReason failureReason = pickedAmount == amount
			? PickupFailureReason.None
			: PickupFailureReason.InvalidRequest;

		return new PickupResult(item.Id, amount, pickedAmount, failureReason);
	}

	private static int CalculateSlotCapacity(InventoryState inventory, InventoryItemDefinition item)
	{
		int totalCapacity = 0;

		foreach (InventorySlotState slot in inventory.Slots)
		{
			if (slot == null)
				continue;

			if (slot.IsEmpty)
			{
				totalCapacity += item.MaxStackSize;
				continue;
			}

			if (slot.ItemId != item.Id)
				continue;

			int freeSpace = Math.Max(0, slot.MaxStackSize - slot.Amount);
			totalCapacity += freeSpace;
		}

		return totalCapacity;
	}

	private static int CalculateWeightCapacity(InventoryState inventory, InventoryItemDefinition item)
	{
		if (item.Weight <= 0f)
			return int.MaxValue;

		float freeWeight = inventory.MaxCarryWeight - inventory.GetCurrentWeight();
		if (freeWeight <= 0f)
			return 0;

		return (int)MathF.Floor(freeWeight / item.Weight);
	}
}
