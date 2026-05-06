using System;

public sealed class EnemySpawnPlan
{
	public int WeakEnemies { get; }
	public int FastEnemies { get; }
	public int ArmoredEnemies { get; }
	public int TotalEnemies => WeakEnemies + FastEnemies + ArmoredEnemies;

	public EnemySpawnPlan(int weakEnemies, int fastEnemies, int armoredEnemies)
	{
		WeakEnemies = Math.Max(0, weakEnemies);
		FastEnemies = Math.Max(0, fastEnemies);
		ArmoredEnemies = Math.Max(0, armoredEnemies);
	}
}

public static class EnemySpawnPlanner
{
	public static EnemySpawnPlan BuildPlan(int dangerLevel, int minutesOutsideBase)
	{
		int danger = Math.Max(0, dangerLevel);
		int minutes = Math.Max(0, minutesOutsideBase);
		int weak = Math.Max(0, danger + minutes / 4);
		int fast = Math.Max(0, danger - 1 + minutes / 6);
		int armored = Math.Max(0, danger - 3 + minutes / 8);
		return new EnemySpawnPlan(weak, fast, armored);
	}
}
