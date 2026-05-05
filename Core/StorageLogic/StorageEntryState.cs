public sealed class StorageEntryState
{
	public string ItemId { get; }
	public int Amount { get; private set; }
	public int MaxStackSize { get; }

	public StorageEntryState(string itemId, int amount, int maxStackSize)
	{
		ItemId = itemId;
		Amount = amount;
		MaxStackSize = maxStackSize;
	}

	public int AddAmount(int amount)
	{
		if (amount <= 0)
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
		if (amount <= 0)
			return;

		Amount -= amount;

		if (Amount < 0)
			Amount = 0;
	}
}
