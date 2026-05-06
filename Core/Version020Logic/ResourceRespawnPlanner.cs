using System;

public sealed class ResourceRespawnPlan
{
	public int NodesToRespawn { get; }
	public int BonusCharges { get; }
	public bool ShouldEscalateDanger { get; }

	public ResourceRespawnPlan(int nodesToRespawn, int bonusCharges, bool shouldEscalateDanger)
	{
		NodesToRespawn = Math.Max(0, nodesToRespawn);
		BonusCharges = Math.Max(0, bonusCharges);
		ShouldEscalateDanger = shouldEscalateDanger;
	}
}

public static class ResourceRespawnPlanner
{
	public static ResourceRespawnPlan BuildPlan(int depletedNodes, int baseLevel, int expeditionNumber)
	{
		int safeDepleted = Math.Max(0, depletedNodes);
		int levelBonus = Math.Max(0, baseLevel - 1);
		int respawn = Math.Max(1, safeDepleted / 2 + levelBonus);
		int bonusCharges = expeditionNumber >= 3 ? 1 : 0;
		bool danger = safeDepleted >= 5 && expeditionNumber >= 2;
		return new ResourceRespawnPlan(respawn, bonusCharges, danger);
	}
}
