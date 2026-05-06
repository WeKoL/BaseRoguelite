using System;

public sealed class EnemyWavePlan030
{
	public int WeakEnemies { get; }
	public int FastEnemies { get; }
	public int ArmoredEnemies { get; }
	public int Total => WeakEnemies + FastEnemies + ArmoredEnemies;
	public EnemyWavePlan030(int weak, int fast, int armored)
	{
		WeakEnemies = Math.Max(0, weak);
		FastEnemies = Math.Max(0, fast);
		ArmoredEnemies = Math.Max(0, armored);
	}
}

public static class EnemyWaveDirector030
{
	public static EnemyWavePlan030 BuildWave(int dangerLevel, int expeditionMinutes, int noiseLevel)
	{
		int danger = Math.Max(1, dangerLevel);
		int timeBonus = Math.Max(0, expeditionMinutes / 5);
		int noiseBonus = Math.Max(0, noiseLevel / 3);
		int total = Math.Clamp(danger + timeBonus + noiseBonus, 1, 12);
		int armored = danger >= 4 ? Math.Max(1, total / 4) : 0;
		int fast = danger >= 2 ? Math.Max(1, total / 3) : 0;
		int weak = Math.Max(0, total - armored - fast);
		return new EnemyWavePlan030(weak, fast, armored);
	}
}
