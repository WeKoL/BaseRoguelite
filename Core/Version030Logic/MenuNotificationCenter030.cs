using System.Collections.Generic;

public sealed class MenuNotificationCenter030
{
	private readonly Queue<string> _messages = new();
	public int Count => _messages.Count;
	public void Push(string message)
	{
		if (!string.IsNullOrWhiteSpace(message)) _messages.Enqueue(message.Trim());
	}
	public string PopOrEmpty() => _messages.Count == 0 ? string.Empty : _messages.Dequeue();
	public IReadOnlyList<string> Drain()
	{
		List<string> result = new();
		while (_messages.Count > 0) result.Add(_messages.Dequeue());
		return result;
	}
}
