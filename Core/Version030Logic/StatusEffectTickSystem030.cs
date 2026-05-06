using System;

public sealed class StatusEffectTickResult030
{
	public int HealthDamage { get; }
	public int StaminaDamage { get; }
	public bool AnyEffectApplied => HealthDamage > 0 || StaminaDamage > 0;
	public StatusEffectTickResult030(int healthDamage, int staminaDamage)
	{
		HealthDamage = Math.Max(0, healthDamage);
		StaminaDamage = Math.Max(0, staminaDamage);
	}
}

public static class StatusEffectTickSystem030
{
	public static StatusEffectTickResult030 Tick(PlayerStatusEffectCollection effects, PlayerStatsState stats, float seconds)
	{
		if (effects == null || stats == null || seconds <= 0f) return new StatusEffectTickResult030(0, 0);
		int healthDamage = 0;
		int staminaDamage = 0;
		if (effects.Has(PlayerStatusEffectKind.Bleeding)) healthDamage += (int)Math.Floor(seconds * 1.5f);
		if (effects.Has(PlayerStatusEffectKind.Poisoned)) healthDamage += (int)Math.Floor(seconds * 1.0f);
		if (effects.Has(PlayerStatusEffectKind.Tired)) staminaDamage += (int)Math.Floor(seconds * 2.0f);
		if (healthDamage > 0) stats.TakeDirectDamage(healthDamage);
		if (staminaDamage > 0) stats.TrySpendStamina(staminaDamage);
		return new StatusEffectTickResult030(healthDamage, staminaDamage);
	}
}
