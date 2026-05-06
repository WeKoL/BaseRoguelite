using System;

public sealed class BaseRaidResult030
{
	public bool Defended { get; }
	public int DamageToBase { get; }
	public int ResourcesLost { get; }
	public BaseRaidResult030(bool defended, int damageToBase, int resourcesLost)
	{
		Defended = defended;
		DamageToBase = Math.Max(0, damageToBase);
		ResourcesLost = Math.Max(0, resourcesLost);
	}
}

public static class BaseRaidResolver030
{
	public static BaseRaidResult030 Resolve(int defensePower, int raidPower, int storageValue)
	{
		int defense = Math.Max(0, defensePower);
		int raid = Math.Max(0, raidPower);
		if (defense >= raid) return new BaseRaidResult030(true, 0, 0);
		int gap = raid - defense;
		return new BaseRaidResult030(false, gap, Math.Min(Math.Max(0, storageValue), gap * 2));
	}
}
