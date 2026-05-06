using System;

public sealed class ToolDurabilityState030
{
	public string ToolItemId { get; }
	public int MaxDurability { get; }
	public int CurrentDurability { get; private set; }
	public bool IsBroken => CurrentDurability <= 0;
	public ToolDurabilityState030(string toolItemId, int maxDurability)
	{
		ToolItemId = toolItemId ?? string.Empty;
		MaxDurability = Math.Max(1, maxDurability);
		CurrentDurability = MaxDurability;
	}
	public int Spend(int amount)
	{
		if (amount <= 0 || IsBroken) return 0;
		int before = CurrentDurability;
		CurrentDurability = Math.Max(0, CurrentDurability - amount);
		return before - CurrentDurability;
	}
	public int Repair(int amount)
	{
		if (amount <= 0) return 0;
		int before = CurrentDurability;
		CurrentDurability = Math.Min(MaxDurability, CurrentDurability + amount);
		return CurrentDurability - before;
	}
}
