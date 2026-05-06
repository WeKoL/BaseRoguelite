using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CombatActor040
{
	public string Id { get; }
	public int MaxHp { get; }
	public int Hp { get; private set; }
	public int Armor { get; private set; }
	public int Damage { get; private set; }
	public float Speed { get; private set; }
	public bool IsAlive => Hp > 0;

	public CombatActor040(string id, int hp, int armor, int damage, float speed = 1f)
	{
		Id = id ?? string.Empty;
		MaxHp = Math.Max(1, hp);
		Hp = MaxHp;
		Armor = Math.Max(0, armor);
		Damage = Math.Max(0, damage);
		Speed = Math.Max(0.1f, speed);
	}

	public int TakeDamage(int rawDamage)
	{
		int final = Math.Max(1, rawDamage - Armor);
		Hp = Math.Max(0, Hp - final);
		return final;
	}

	public void ApplyEquipment(EquipmentCore040 equipment)
	{
		Armor += equipment?.Armor ?? 0;
		Damage += equipment?.Damage ?? 0;
	}
}

public sealed class CombatCore040
{
	public CombatResult040 Attack(CombatActor040 attacker, CombatActor040 defender, int bonusDamage = 0)
	{
		if (attacker == null || defender == null) return CombatResult040.Fail("actor_missing");
		if (!attacker.IsAlive) return CombatResult040.Fail("attacker_dead");
		if (!defender.IsAlive) return CombatResult040.Fail("defender_dead");
		int dealt = defender.TakeDamage(attacker.Damage + Math.Max(0, bonusDamage));
		return new CombatResult040(true, string.Empty, dealt, !defender.IsAlive);
	}
}

public sealed class CombatResult040
{
	public bool Succeeded { get; }
	public string Error { get; }
	public int DamageDealt { get; }
	public bool TargetKilled { get; }
	public CombatResult040(bool succeeded, string error, int damageDealt, bool targetKilled) { Succeeded = succeeded; Error = error ?? string.Empty; DamageDealt = Math.Max(0, damageDealt); TargetKilled = targetKilled; }
	public static CombatResult040 Fail(string error) => new(false, error, 0, false);
}

public enum EnemyDecision040
{
	Idle,
	Chase,
	Attack,
	Retreat
}

public sealed class EnemyAiCore040
{
	public EnemyDecision040 Decide(float distanceToPlayer, int enemyHp, int playerHp, bool playerInBase)
	{
		if (playerInBase) return EnemyDecision040.Retreat;
		if (enemyHp < 10 && playerHp > enemyHp) return EnemyDecision040.Retreat;
		if (distanceToPlayer <= 24f) return EnemyDecision040.Attack;
		if (distanceToPlayer <= 180f) return EnemyDecision040.Chase;
		return EnemyDecision040.Idle;
	}
}

public sealed class EnemyWaveCore040
{
	public IReadOnlyList<CombatActor040> CreateWave(int dangerLevel, int day)
	{
		int count = Math.Max(1, dangerLevel + day / 3);
		List<CombatActor040> enemies = new();
		for (int i = 0; i < count; i++)
			enemies.Add(new CombatActor040($"enemy_{day}_{i}", 20 + dangerLevel * 5, dangerLevel / 2, 4 + dangerLevel, 1f + dangerLevel * 0.05f));
		return enemies;
	}
}

public sealed class AmmoCore040
{
	private readonly Dictionary<string, int> _ammo = new(StringComparer.OrdinalIgnoreCase);
	public void Add(string ammoId, int amount) { if (!string.IsNullOrWhiteSpace(ammoId)) _ammo[ammoId] = Get(ammoId) + Math.Max(0, amount); }
	public int Get(string ammoId) => _ammo.TryGetValue(ammoId ?? string.Empty, out int amount) ? amount : 0;
	public bool Spend(string ammoId, int amount) { if (Get(ammoId) < amount) return false; _ammo[ammoId] -= amount; return true; }
}
