public sealed class FeedbackEvent
{
	public string Text { get; }
	public float LifetimeSeconds { get; }
	public bool IsImportant { get; }
	public FeedbackEvent(string text, float lifetimeSeconds = 1.5f, bool isImportant = false) { Text = text ?? string.Empty; LifetimeSeconds = lifetimeSeconds <= 0f ? 1.5f : lifetimeSeconds; IsImportant = isImportant; }
}
