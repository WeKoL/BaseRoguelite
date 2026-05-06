using System;

public sealed class RangedAttackResult030
{
	public bool Fired { get; }
	public bool Hit { get; }
	public int AmmoSpent { get; }
	public int DamageDealt { get; }
	public string Reason { get; }
	public RangedAttackResult030(bool fired, bool hit, int ammoSpent, int damageDealt, string reason = "")
	{
		Fired = fired;
		Hit = hit;
		AmmoSpent = Math.Max(0, ammoSpent);
		DamageDealt = Math.Max(0, damageDealt);
		Reason = reason ?? string.Empty;
	}
}

public static class RangedAttackLogic030
{
	public static RangedAttackResult030 Shoot(EnemyState enemy, int ammoAvailable, int damage, float distance, float maxRange)
	{
		if (enemy == null || enemy.IsDead) return new RangedAttackResult030(false, false, 0, 0, "no_target");
		if (ammoAvailable <= 0) return new RangedAttackResult030(false, false, 0, 0, "no_ammo");
		if (distance > maxRange) return new RangedAttackResult030(true, false, 1, 0, "out_of_range");
		int dealt = enemy.TakeDamage(damage);
		return new RangedAttackResult030(true, dealt > 0, 1, dealt, dealt > 0 ? "hit" : "blocked");
	}
}
