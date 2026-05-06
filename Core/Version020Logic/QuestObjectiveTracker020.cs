using System;
using System.Collections.Generic;

public sealed class QuestObjectiveProgress020
{
	public string ObjectiveId { get; }
	public int Current { get; private set; }
	public int Required { get; }
	public bool IsCompleted => Current >= Required;

	public QuestObjectiveProgress020(string objectiveId, int required)
	{
		ObjectiveId = objectiveId ?? string.Empty;
		Required = Math.Max(1, required);
	}

	public void AddProgress(int amount)
	{
		Current = Math.Min(Required, Current + Math.Max(0, amount));
	}
}

public sealed class QuestObjectiveTracker020
{
	private readonly Dictionary<string, QuestObjectiveProgress020> _objectives = new();
	public IReadOnlyDictionary<string, QuestObjectiveProgress020> Objectives => _objectives;

	public QuestObjectiveProgress020 Register(string objectiveId, int required)
	{
		if (!_objectives.TryGetValue(objectiveId, out QuestObjectiveProgress020 objective))
		{
			objective = new QuestObjectiveProgress020(objectiveId, required);
			_objectives[objectiveId] = objective;
		}
		return objective;
	}

	public bool AddProgress(string objectiveId, int amount)
	{
		if (!_objectives.TryGetValue(objectiveId, out QuestObjectiveProgress020 objective)) return false;
		objective.AddProgress(amount);
		return objective.IsCompleted;
	}

	public bool AreAllCompleted()
	{
		if (_objectives.Count == 0) return false;
		foreach (QuestObjectiveProgress020 objective in _objectives.Values)
			if (!objective.IsCompleted) return false;
		return true;
	}
}
