#nullable enable

public sealed class InventorySlotState
{
	public string? ItemId { get; private set; }
	public int Amount { get; private set; }
	public int MaxStackSize { get; private set; }
	public float WeightPerUnit { get; private set; }

	public bool IsEmpty => string.IsNullOrWhiteSpace(ItemId) || Amount <= 0;

	public void SetItem(string itemId, int amount, int maxStackSize, float weightPerUnit)
	{
		ItemId = itemId;
		Amount = amount;
		MaxStackSize = maxStackSize;
		WeightPerUnit = weightPerUnit;
	}

	public int AddAmount(int amount)
	{
		if (amount <= 0 || IsEmpty)
			return 0;

		int freeSpace = MaxStackSize - Amount;
		if (freeSpace <= 0)
			return 0;

		int added = System.Math.Min(freeSpace, amount);
		Amount += added;
		return added;
	}

	public void RemoveAmount(int amount)
	{
		if (amount <= 0 || IsEmpty)
			return;

		Amount -= amount;

		if (Amount <= 0)
		{
			ItemId = null;
			Amount = 0;
			MaxStackSize = 0;
			WeightPerUnit = 0f;
		}
	}
}
