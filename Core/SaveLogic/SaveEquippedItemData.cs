public sealed class SaveEquippedItemData
{
	public string ItemId { get; set; } = string.Empty;
	public string SlotId { get; set; } = string.Empty;
	public int CurrentDurability { get; set; }
	public int MaxDurability { get; set; }
}
