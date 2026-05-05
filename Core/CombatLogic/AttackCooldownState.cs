public sealed class AttackCooldownState
{
	public int CooldownMilliseconds { get; }
	public int RemainingMilliseconds { get; private set; }
	public bool IsReady => RemainingMilliseconds <= 0;
	public AttackCooldownState(int cooldownMilliseconds) { CooldownMilliseconds = cooldownMilliseconds <= 0 ? 1 : cooldownMilliseconds; }
	public bool TryUse() { if (!IsReady) return false; RemainingMilliseconds = CooldownMilliseconds; return true; }
	public void Tick(int milliseconds) { if (milliseconds <= 0) return; RemainingMilliseconds = System.Math.Max(0, RemainingMilliseconds - milliseconds); }
}
