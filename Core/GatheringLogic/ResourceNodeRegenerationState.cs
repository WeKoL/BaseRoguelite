public sealed class ResourceNodeRegenerationState
{
	public int MaxCharges { get; }
	public int CurrentCharges { get; private set; }
	public int SecondsPerCharge { get; }
	private int _accumulatedSeconds;
	public ResourceNodeRegenerationState(int maxCharges, int currentCharges, int secondsPerCharge) { MaxCharges = maxCharges <= 0 ? 1 : maxCharges; CurrentCharges = System.Math.Clamp(currentCharges, 0, MaxCharges); SecondsPerCharge = secondsPerCharge <= 0 ? 60 : secondsPerCharge; }
	public int SpendOneCharge() { if (CurrentCharges <= 0) return 0; CurrentCharges--; return 1; }
	public int Tick(int seconds) { if (seconds <= 0 || CurrentCharges >= MaxCharges) return 0; _accumulatedSeconds += seconds; int restored = 0; while (_accumulatedSeconds >= SecondsPerCharge && CurrentCharges < MaxCharges) { _accumulatedSeconds -= SecondsPerCharge; CurrentCharges++; restored++; } return restored; }
}
