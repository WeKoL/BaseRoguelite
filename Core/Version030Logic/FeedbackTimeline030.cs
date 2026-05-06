using System.Collections.Generic;
using System.Linq;

public sealed class FeedbackTimelineEvent030
{
	public float TimeSeconds { get; }
	public string Text { get; }
	public FeedbackTimelineEvent030(float timeSeconds, string text)
	{
		TimeSeconds = System.Math.Max(0f, timeSeconds);
		Text = text ?? string.Empty;
	}
}

public sealed class FeedbackTimeline030
{
	private readonly List<FeedbackTimelineEvent030> _events = new();
	public IReadOnlyList<FeedbackTimelineEvent030> Events => _events;
	public void Add(float timeSeconds, string text)
	{
		if (!string.IsNullOrWhiteSpace(text)) _events.Add(new FeedbackTimelineEvent030(timeSeconds, text));
	}
	public IReadOnlyList<FeedbackTimelineEvent030> GetEventsUpTo(float timeSeconds)
	{
		return _events.Where(e => e.TimeSeconds <= timeSeconds).OrderBy(e => e.TimeSeconds).ToList();
	}
}
