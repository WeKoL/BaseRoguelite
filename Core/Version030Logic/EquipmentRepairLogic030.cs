using System;

public sealed class EquipmentRepairQuote030
{
	public string RequiredItemId { get; }
	public int RequiredAmount { get; }
	public int DurabilityRestored { get; }
	public bool IsAlreadyFull => DurabilityRestored <= 0;
	public EquipmentRepairQuote030(string requiredItemId, int requiredAmount, int durabilityRestored)
	{
		RequiredItemId = requiredItemId ?? "repair_kit";
		RequiredAmount = Math.Max(0, requiredAmount);
		DurabilityRestored = Math.Max(0, durabilityRestored);
	}
}

public static class EquipmentRepairLogic030
{
	public static EquipmentRepairQuote030 Quote(int currentDurability, int maxDurability, int repairPowerPerKit = 35)
	{
		int missing = Math.Max(0, maxDurability - currentDurability);
		if (missing <= 0) return new EquipmentRepairQuote030("repair_kit", 0, 0);
		int power = Math.Max(1, repairPowerPerKit);
		int kits = (int)Math.Ceiling(missing / (double)power);
		return new EquipmentRepairQuote030("repair_kit", kits, Math.Min(missing, kits * power));
	}

	public static int ApplyRepair(int currentDurability, int maxDurability, int kitsUsed, int repairPowerPerKit = 35)
	{
		if (maxDurability <= 0) return 0;
		int restored = Math.Max(0, kitsUsed) * Math.Max(1, repairPowerPerKit);
		return Math.Min(maxDurability, Math.Max(0, currentDurability) + restored);
	}
}
