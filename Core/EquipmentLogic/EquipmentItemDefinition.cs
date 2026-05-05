using System;

public sealed class EquipmentItemDefinition
{
	public string ItemId { get; }
	public string DisplayName { get; }
	public EquipmentSlotId SlotId { get; }
	public int MaxDurability { get; }

	public EquipmentItemDefinition(
		string itemId,
		EquipmentSlotId slotId,
		int maxDurability,
		string displayName = null)
	{
		ItemId = itemId ?? string.Empty;
		DisplayName = string.IsNullOrWhiteSpace(displayName) ? ItemId : displayName;
		SlotId = slotId;
		MaxDurability = Math.Max(0, maxDurability);
	}

	public bool IsValid()
	{
		return !string.IsNullOrWhiteSpace(ItemId) && MaxDurability > 0;
	}
}
