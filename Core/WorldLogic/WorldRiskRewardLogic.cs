public static class WorldRiskRewardLogic
{
	public static int GetLootBonusPercent(WorldZoneState zone) { if (zone == null) return 0; return zone.DangerLevel * 10; }
	public static int GetEnemySpawnWeight(WorldZoneState zone) { if (zone == null) return 0; return 1 + zone.DangerLevel * 2; }
}
