public sealed class EquipmentLoadoutScore
{
	public int EquippedCount { get; }
	public int BrokenCount { get; }
	public int TotalDurability { get; }
	public int MaxDurability { get; }
	public float DurabilityRatio => MaxDurability <= 0 ? 0f : TotalDurability / (float)MaxDurability;
	public EquipmentLoadoutScore(int equippedCount, int brokenCount, int totalDurability, int maxDurability)
	{ EquippedCount = equippedCount < 0 ? 0 : equippedCount; BrokenCount = brokenCount < 0 ? 0 : brokenCount; TotalDurability = totalDurability < 0 ? 0 : totalDurability; MaxDurability = maxDurability < 0 ? 0 : maxDurability; }
}
