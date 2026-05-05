using Godot;

public sealed class EquippedItemViewData
{
	public EquipmentSlotId SlotId { get; }
	public string DisplayName { get; }
	public ItemData SourceItem { get; }
	public Texture2D Icon { get; }
	public int CurrentDurability { get; }
	public int MaxDurability { get; }

	public EquippedItemViewData(
		EquipmentSlotId slotId,
		string displayName,
		ItemData sourceItem,
		Texture2D icon,
		int currentDurability,
		int maxDurability)
	{
		SlotId = slotId;
		DisplayName = displayName ?? string.Empty;
		SourceItem = sourceItem;
		Icon = icon;
		CurrentDurability = currentDurability;
		MaxDurability = maxDurability;
	}
}
