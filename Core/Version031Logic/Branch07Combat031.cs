using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ThreatSnapshot031
{
	public int EnemyCount { get; }
	public int HighestDamage { get; }
	public float NearestDistance { get; }
	public int ThreatScore => EnemyCount * 5 + HighestDamage * 2 + (NearestDistance < 80f ? 10 : 0);
	public bool ShouldRetreat => ThreatScore >= 35;

	public ThreatSnapshot031(int enemyCount, int highestDamage, float nearestDistance)
	{
		EnemyCount = Math.Max(0, enemyCount);
		HighestDamage = Math.Max(0, highestDamage);
		NearestDistance = Math.Max(0, nearestDistance);
	}
}

public static class CombatEncounterPlanner031
{
	public static string ChooseTactic(ThreatSnapshot031 threat, int playerHealth, bool hasRangedWeapon)
	{
		if (threat == null) return "hold";
		if (playerHealth <= 25 || threat.ShouldRetreat) return "retreat";
		if (hasRangedWeapon && threat.NearestDistance > 60f) return "kite";
		return threat.EnemyCount > 1 ? "focus_weakest" : "melee_attack";
	}
}

public sealed class AmmoReserve031
{
	private readonly Dictionary<string, int> _ammo = new();
	public void Add(string ammoId, int amount)
	{
		if (string.IsNullOrWhiteSpace(ammoId) || amount <= 0) return;
		_ammo[ammoId] = Get(ammoId) + amount;
	}

	public bool TrySpend(string ammoId, int amount)
	{
		if (Get(ammoId) < amount) return false;
		_ammo[ammoId] -= amount;
		return true;
	}

	public int Get(string ammoId) => _ammo.TryGetValue(ammoId ?? string.Empty, out int value) ? value : 0;
}
