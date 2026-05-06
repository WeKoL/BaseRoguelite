using System;
using System.Collections.Generic;
using System.Linq;

public sealed class QuestObjective040
{
	public string EventId { get; }
	public int RequiredAmount { get; }
	public QuestObjective040(string eventId, int requiredAmount) { EventId = eventId ?? string.Empty; RequiredAmount = Math.Max(1, requiredAmount); }
}

public sealed class QuestDefinition040
{
	public string Id { get; }
	public string Title { get; }
	public IReadOnlyList<QuestObjective040> Objectives { get; }
	public IReadOnlyDictionary<string, int> Rewards { get; }
	public QuestDefinition040(string id, string title, IReadOnlyList<QuestObjective040> objectives, IReadOnlyDictionary<string, int> rewards)
	{
		Id = id ?? string.Empty;
		Title = string.IsNullOrWhiteSpace(title) ? Id : title;
		Objectives = objectives ?? Array.Empty<QuestObjective040>();
		Rewards = rewards ?? new Dictionary<string, int>();
	}
}

public sealed class QuestTracker040
{
	private readonly Dictionary<string, QuestDefinition040> _quests = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, int> _progress = new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _completed = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyCollection<string> Completed => _completed;
	public void AddQuest(QuestDefinition040 quest) { if (quest != null) _quests[quest.Id] = quest; }
	public void RecordEvent(string eventId, int amount = 1) { if (!string.IsNullOrWhiteSpace(eventId)) _progress[eventId] = GetEventProgress(eventId) + Math.Max(0, amount); RefreshCompleted(); }
	public int GetEventProgress(string eventId) => _progress.TryGetValue(eventId ?? string.Empty, out int amount) ? amount : 0;
	public bool IsCompleted(string questId) => _completed.Contains(questId ?? string.Empty);

	public IReadOnlyDictionary<string, int> ClaimReward(string questId)
	{
		if (!IsCompleted(questId) || !_quests.TryGetValue(questId, out QuestDefinition040 quest)) return new Dictionary<string, int>();
		return quest.Rewards;
	}

	private void RefreshCompleted()
	{
		foreach (QuestDefinition040 quest in _quests.Values)
		{
			if (_completed.Contains(quest.Id)) continue;
			if (quest.Objectives.All(x => GetEventProgress(x.EventId) >= x.RequiredAmount)) _completed.Add(quest.Id);
		}
	}

	public static QuestTracker040 CreateTutorial()
	{
		QuestTracker040 tracker = new();
		tracker.AddQuest(new QuestDefinition040("first_gather", "Собрать первые ресурсы", new[] { new QuestObjective040("gather_resource", 5) }, new Dictionary<string, int> { ["wood"] = 3 }));
		tracker.AddQuest(new QuestDefinition040("first_craft", "Сделать аптечку", new[] { new QuestObjective040("craft_medkit", 1) }, new Dictionary<string, int> { ["herb"] = 2 }));
		tracker.AddQuest(new QuestDefinition040("secure_base", "Укрепить базу", new[] { new QuestObjective040("upgrade_walls", 1) }, new Dictionary<string, int> { ["ammo_basic"] = 10 }));
		return tracker;
	}
}
