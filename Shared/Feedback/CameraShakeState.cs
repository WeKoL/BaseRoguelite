public sealed class CameraShakeState
{
	public float Strength { get; private set; }
	public float TimeLeft { get; private set; }
	public bool IsActive => TimeLeft > 0f && Strength > 0f;
	public void Start(float strength, float duration) { Strength = strength < 0f ? 0f : strength; TimeLeft = duration < 0f ? 0f : duration; }
	public void Tick(float delta) { if (delta <= 0f) return; TimeLeft = System.MathF.Max(0f, TimeLeft - delta); if (TimeLeft <= 0f) Strength = 0f; }
}
