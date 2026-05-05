public sealed class DamageFeedbackPlan
{
	public int Damage { get; }
	public float ShakeStrength { get; }
	public bool ShowCriticalText { get; }
	public DamageFeedbackPlan(int damage, float shakeStrength, bool showCriticalText) { Damage = damage < 0 ? 0 : damage; ShakeStrength = shakeStrength < 0f ? 0f : shakeStrength; ShowCriticalText = showCriticalText; }
}
