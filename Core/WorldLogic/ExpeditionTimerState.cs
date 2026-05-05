public sealed class ExpeditionTimerState
{
	public int MaxSeconds { get; }
	public int ElapsedSeconds { get; private set; }
	public int RemainingSeconds => System.Math.Max(0, MaxSeconds - ElapsedSeconds);
	public bool IsExpired => RemainingSeconds <= 0;
	public ExpeditionTimerState(int maxSeconds) { MaxSeconds = maxSeconds <= 0 ? 1 : maxSeconds; }
	public void Tick(int seconds) { if (seconds <= 0) return; ElapsedSeconds = System.Math.Min(MaxSeconds, ElapsedSeconds + seconds); }
}
