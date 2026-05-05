public sealed class InventorySlotSnapshot
{
	public int SlotIndex { get; }
	public string ItemId { get; }
	public int Amount { get; }
	public int MaxStackSize { get; }
	public float WeightPerUnit { get; }

	public InventorySlotSnapshot(string itemId, int amount, int maxStackSize, float weightPerUnit)
		: this(-1, itemId, amount, maxStackSize, weightPerUnit) { }

	public InventorySlotSnapshot(int slotIndex, string itemId, int amount, int maxStackSize, float weightPerUnit)
	{
		SlotIndex = slotIndex;
		ItemId = itemId ?? string.Empty;
		Amount = amount < 0 ? 0 : amount;
		MaxStackSize = maxStackSize <= 0 ? 1 : maxStackSize;
		WeightPerUnit = weightPerUnit < 0f ? 0f : weightPerUnit;
	}
}
