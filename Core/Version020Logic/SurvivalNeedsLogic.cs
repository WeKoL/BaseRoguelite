using System;

public sealed class SurvivalNeedsUpdateResult
{
	public int FoodConsumed { get; }
	public int WaterConsumed { get; }
	public int HealthLost { get; }
	public bool IsCritical { get; }

	public SurvivalNeedsUpdateResult(int foodConsumed, int waterConsumed, int healthLost, bool isCritical)
	{
		FoodConsumed = Math.Max(0, foodConsumed);
		WaterConsumed = Math.Max(0, waterConsumed);
		HealthLost = Math.Max(0, healthLost);
		IsCritical = isCritical;
	}
}

public static class SurvivalNeedsLogic
{
	public static SurvivalNeedsUpdateResult Tick(SurvivalNeedsState needs, PlayerStatsState stats, float seconds, bool isInsideBase)
	{
		if (needs == null || seconds <= 0f)
			return new SurvivalNeedsUpdateResult(0, 0, 0, false);

		float multiplier = isInsideBase ? 0.35f : 1.0f;
		int foodToConsume = Math.Max(0, (int)Math.Floor(seconds * 0.18f * multiplier));
		int waterToConsume = Math.Max(0, (int)Math.Floor(seconds * 0.24f * multiplier));

		needs.Consume(foodToConsume, waterToConsume);

		int healthLost = 0;
		bool critical = needs.Food <= 0 || needs.Water <= 0;
		if (critical && stats != null)
			healthLost = stats.TakeDirectDamage(Math.Max(1, (int)Math.Floor(seconds * 0.25f)));

		return new SurvivalNeedsUpdateResult(foodToConsume, waterToConsume, healthLost, critical);
	}
}
