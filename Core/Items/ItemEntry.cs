using Godot;

public partial class ItemEntry : RefCounted
{
	public ItemData Item { get; private set; }
	public int Amount { get; private set; }

	public ItemEntry(ItemData item, int amount)
	{
		Item = item;
		Amount = Mathf.Max(0, amount);
	}

	public bool IsEmpty()
	{
		return Item == null || Amount <= 0;
	}

	public bool CanMergeWith(ItemEntry other)
	{
		if (other == null || Item == null || other.Item == null)
			return false;

		return Item == other.Item;
	}

	public int AddAmount(int value)
	{
		if (Item == null || value <= 0)
			return 0;

		int maxStack = Mathf.Max(1, Item.MaxStackSize);
		int freeSpace = Mathf.Max(0, maxStack - Amount);
		int added = Mathf.Min(freeSpace, value);

		Amount += added;
		return added;
	}

	public int RemoveAmount(int value)
	{
		if (value <= 0)
			return 0;

		int removed = Mathf.Min(Amount, value);
		Amount -= removed;
		return removed;
	}

	public ItemEntry Clone()
	{
		return new ItemEntry(Item, Amount);
	}
}
