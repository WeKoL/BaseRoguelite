using System.Collections.Generic;
public sealed class PlayerStatusEffectCollection
{
	private readonly List<PlayerStatusEffectState> _effects = new();
	public IReadOnlyList<PlayerStatusEffectState> Effects => _effects;
	public void Add(PlayerStatusEffectState effect) { if (effect != null && effect.IsActive) _effects.Add(effect); }
	public int TickAll(PlayerStatsState stats, int seconds) { if (seconds <= 0) return 0; int damage = 0; for (int i = _effects.Count - 1; i >= 0; i--) { damage += _effects[i].Tick(stats, seconds); if (!_effects[i].IsActive) _effects.RemoveAt(i); } return damage; }
	public bool Has(PlayerStatusEffectKind kind) { foreach (PlayerStatusEffectState e in _effects) if (e.IsActive && e.Kind == kind) return true; return false; }
}
