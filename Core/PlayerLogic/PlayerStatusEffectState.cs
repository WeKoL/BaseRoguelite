public sealed class PlayerStatusEffectState
{
	public PlayerStatusEffectKind Kind { get; }
	public int DurationSeconds { get; private set; }
	public int DamagePerTick { get; }
	public bool IsActive => Kind != PlayerStatusEffectKind.None && DurationSeconds > 0;
	public PlayerStatusEffectState(PlayerStatusEffectKind kind, int durationSeconds, int damagePerTick = 0) { Kind = kind; DurationSeconds = durationSeconds < 0 ? 0 : durationSeconds; DamagePerTick = damagePerTick < 0 ? 0 : damagePerTick; }
	public int Tick(PlayerStatsState stats, int seconds)
	{
		if (!IsActive || seconds <= 0) return 0; int elapsed = System.Math.Min(DurationSeconds, seconds); DurationSeconds -= elapsed; if (stats == null || DamagePerTick <= 0) return 0; return stats.TakeDirectDamage(DamagePerTick * elapsed);
	}
}
