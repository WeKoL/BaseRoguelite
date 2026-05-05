public static class EquipmentLoadoutScoreCalculator
{
	public static EquipmentLoadoutScore Calculate(EquipmentState equipment)
	{
		if (equipment == null) return new EquipmentLoadoutScore(0,0,0,0);
		int equipped=0, broken=0, current=0, max=0;
		foreach (EquipmentSlotId slotId in System.Enum.GetValues(typeof(EquipmentSlotId)))
		{
			EquippedItemState item = equipment.GetEquippedItem(slotId); if (item == null) continue;
			equipped++; if (item.IsBroken) broken++; current += item.CurrentDurability; max += item.MaxDurability;
		}
		return new EquipmentLoadoutScore(equipped, broken, current, max);
	}
}
