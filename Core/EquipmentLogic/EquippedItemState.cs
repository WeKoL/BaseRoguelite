using System;

public sealed class EquippedItemState
{
	public string ItemId { get; }
	public string BaseDisplayName { get; }
	public string DisplayName => EquipmentModifierCatalog.BuildDisplayName(BaseDisplayName, ModifierId);

	public EquipmentSlotId SlotId { get; }
	public EquipmentModifierId ModifierId { get; }

	public int MaxDurability { get; }
	public int CurrentDurability { get; private set; }

	public bool IsBroken => CurrentDurability <= 0;

	public EquippedItemState(
		string itemId,
		EquipmentSlotId slotId,
		int maxDurability,
		int currentDurability,
		string baseDisplayName = null,
		EquipmentModifierId modifierId = EquipmentModifierId.None)
	{
		ItemId = itemId ?? string.Empty;
		BaseDisplayName = string.IsNullOrWhiteSpace(baseDisplayName) ? ItemId : baseDisplayName;
		SlotId = slotId;
		ModifierId = modifierId;
		MaxDurability = Math.Max(0, maxDurability);
		CurrentDurability = Math.Clamp(currentDurability, 0, MaxDurability);
	}

	public void ReduceDurability(int amount)
	{
		if (amount <= 0)
			return;

		CurrentDurability = Math.Max(0, CurrentDurability - amount);
	}
}
