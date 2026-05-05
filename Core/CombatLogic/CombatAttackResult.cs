public sealed class CombatAttackResult
{
	public bool Succeeded { get; }
	public string Reason { get; }
	public int DamageDealt { get; }
	public bool TargetKilled { get; }
	private CombatAttackResult(bool succeeded, string reason, int damageDealt, bool targetKilled) { Succeeded = succeeded; Reason = reason ?? string.Empty; DamageDealt = damageDealt < 0 ? 0 : damageDealt; TargetKilled = targetKilled; }
	public static CombatAttackResult Success(int damageDealt, bool targetKilled) => new(true, string.Empty, damageDealt, targetKilled);
	public static CombatAttackResult Fail(string reason) => new(false, reason, 0, false);
}
