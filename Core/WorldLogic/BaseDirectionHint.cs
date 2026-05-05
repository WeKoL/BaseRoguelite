public sealed class BaseDirectionHint
{
	public float DeltaX { get; }
	public float DeltaY { get; }
	public float Distance { get; }
	public string Text { get; }
	public BaseDirectionHint(float deltaX, float deltaY, float distance, string text) { DeltaX = deltaX; DeltaY = deltaY; Distance = distance < 0f ? 0f : distance; Text = text ?? string.Empty; }
}
