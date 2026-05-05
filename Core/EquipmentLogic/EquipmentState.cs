using System.Collections.Generic;

public sealed class EquipmentState
{
	private readonly Dictionary<EquipmentSlotId, EquippedItemState> _equipped = new();

	public EquippedItemState GetEquippedItem(EquipmentSlotId slotId)
	{
		_equipped.TryGetValue(slotId, out EquippedItemState item);
		return item;
	}

	public EquipResult EquipToSlot(EquipmentSlotId slotId, EquipmentItemDefinition item)
	{
		return EquipToSlot(slotId, item, EquipmentModifierId.None);
	}

	public EquipResult EquipToSlot(
		EquipmentSlotId slotId,
		EquipmentItemDefinition item,
		EquipmentModifierId modifierId)
	{
		if (item == null || !item.IsValid())
			return EquipResult.Fail(EquipmentEquipFailureReason.InvalidItem);

		if (item.SlotId != slotId)
			return EquipResult.Fail(EquipmentEquipFailureReason.SlotMismatch);

		if (!EquipmentModifierCatalog.IsAllowedForSlot(modifierId, slotId))
			return EquipResult.Fail(EquipmentEquipFailureReason.ModifierNotAllowed);

		EquippedItemState replacedItem = GetEquippedItem(slotId);

		int adjustedMaxDurability = EquipmentModifierCatalog.ApplyDurabilityModifier(
			modifierId,
			item.MaxDurability);

		EquippedItemState equippedItem = new EquippedItemState(
			item.ItemId,
			item.SlotId,
			adjustedMaxDurability,
			adjustedMaxDurability,
			item.DisplayName,
			modifierId);

		_equipped[slotId] = equippedItem;

		return EquipResult.Success(equippedItem, replacedItem);
	}

	public EquippedItemState Unequip(EquipmentSlotId slotId)
	{
		EquippedItemState equippedItem = GetEquippedItem(slotId);
		if (equippedItem == null)
			return null;

		_equipped.Remove(slotId);
		return equippedItem;
	}

	public int ReduceDurability(EquipmentSlotId slotId, int amount)
	{
		if (amount <= 0)
			return GetCurrentDurability(slotId);

		EquippedItemState equippedItem = GetEquippedItem(slotId);
		if (equippedItem == null)
			return 0;

		equippedItem.ReduceDurability(amount);
		return equippedItem.CurrentDurability;
	}

	public int GetCurrentDurability(EquipmentSlotId slotId)
	{
		EquippedItemState equippedItem = GetEquippedItem(slotId);
		return equippedItem?.CurrentDurability ?? 0;
	}
}
