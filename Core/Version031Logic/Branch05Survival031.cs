using System;
using System.Collections.Generic;

public sealed class RestPlan031
{
	public float Hours { get; }
	public int HealthRestore { get; }
	public int FoodCost { get; }
	public int WaterCost { get; }
	public bool IsAffordable { get; }

	public RestPlan031(float hours, int healthRestore, int foodCost, int waterCost, bool isAffordable)
	{
		Hours = Math.Max(0, hours);
		HealthRestore = Math.Max(0, healthRestore);
		FoodCost = Math.Max(0, foodCost);
		WaterCost = Math.Max(0, waterCost);
		IsAffordable = isAffordable;
	}
}

public static class RestPlanner031
{
	public static RestPlan031 Plan(float hours, int currentFood, int currentWater)
	{
		int roundedHours = Math.Max(0, (int)Math.Ceiling(hours));
		int foodCost = roundedHours * 3;
		int waterCost = roundedHours * 4;
		int healthRestore = roundedHours * 12;
		return new RestPlan031(hours, healthRestore, foodCost, waterCost, currentFood >= foodCost && currentWater >= waterCost);
	}
}

public sealed class ExposureState031
{
	public float Cold { get; private set; }
	public float Radiation { get; private set; }
	public bool IsDangerous => Cold >= 80f || Radiation >= 80f;

	public void Tick(float seconds, float coldRate, float radiationRate, float resistance)
	{
		float multiplier = Math.Clamp(1f - resistance, 0.05f, 1f);
		Cold = Math.Clamp(Cold + Math.Max(0, seconds) * coldRate * multiplier, 0f, 100f);
		Radiation = Math.Clamp(Radiation + Math.Max(0, seconds) * radiationRate * multiplier, 0f, 100f);
	}

	public IReadOnlyList<string> GetWarnings()
	{
		List<string> warnings = new();
		if (Cold >= 50f) warnings.Add("cold");
		if (Radiation >= 50f) warnings.Add("radiation");
		return warnings;
	}
}
