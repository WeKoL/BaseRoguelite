using System;
using System.Collections.Generic;
using System.Linq;

public enum FeedbackKind040
{
	Info,
	Pickup,
	Craft,
	Damage,
	Death,
	Quest,
	BaseUpgrade,
	Error
}

public sealed class FeedbackEvent040
{
	public FeedbackKind040 Kind { get; }
	public string Text { get; }
	public float Intensity { get; }
	public FeedbackEvent040(FeedbackKind040 kind, string text, float intensity = 1f) { Kind = kind; Text = text ?? string.Empty; Intensity = Math.Clamp(intensity, 0f, 10f); }
}

public sealed class FeedbackBus040
{
	private readonly Queue<FeedbackEvent040> _events = new();
	public int Count => _events.Count;
	public void Push(FeedbackKind040 kind, string text, float intensity = 1f) => _events.Enqueue(new FeedbackEvent040(kind, text, intensity));
	public IReadOnlyList<FeedbackEvent040> Drain(int max = 10)
	{
		List<FeedbackEvent040> result = new();
		while (_events.Count > 0 && result.Count < max)
			result.Add(_events.Dequeue());
		return result;
	}
}

public sealed class FeedbackPlanner040
{
	public FeedbackEvent040 ForDamage(int damage, bool killed)
	{
		if (killed) return new FeedbackEvent040(FeedbackKind040.Death, "Цель уничтожена", 8f);
		return new FeedbackEvent040(FeedbackKind040.Damage, $"Урон {damage}", Math.Clamp(damage / 5f, 1f, 6f));
	}

	public FeedbackEvent040 ForCraft(string itemId) => new(FeedbackKind040.Craft, $"Создано: {itemId}", 2f);
	public FeedbackEvent040 ForQuest(string questId) => new(FeedbackKind040.Quest, $"Цель выполнена: {questId}", 3f);
}
