public sealed class StorageCategorySummary
{
	public ItemCategory Category { get; }
	public int StackCount { get; }
	public int TotalAmount { get; }
	public StorageCategorySummary(ItemCategory category, int stackCount, int totalAmount)
	{ Category = category; StackCount = stackCount < 0 ? 0 : stackCount; TotalAmount = totalAmount < 0 ? 0 : totalAmount; }
}
