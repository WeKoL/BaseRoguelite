public sealed class LootTableEntry
{
	public string ItemId { get; }
	public int MinAmount { get; }
	public int MaxAmount { get; }
	public int Weight { get; }
	public LootTableEntry(string itemId, int minAmount, int maxAmount, int weight) { ItemId = itemId ?? string.Empty; MinAmount = minAmount <= 0 ? 1 : minAmount; MaxAmount = System.Math.Max(MinAmount, maxAmount); Weight = weight <= 0 ? 1 : weight; }
}
