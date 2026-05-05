using System.Collections.Generic;
public sealed class FeedbackQueue
{
	private readonly Queue<FeedbackEvent> _events = new();
	public int Count => _events.Count;
	public void Enqueue(FeedbackEvent feedback) { if (feedback != null) _events.Enqueue(feedback); }
	public FeedbackEvent Dequeue() { return _events.Count == 0 ? null : _events.Dequeue(); }
}
