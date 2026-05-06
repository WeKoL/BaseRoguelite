using System.Collections.Generic;

public sealed class QuestEventRouter030
{
	private readonly Dictionary<string, List<string>> _eventToObjectives = new();
	public bool Register(string eventId, string objectiveId)
	{
		if (string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(objectiveId)) return false;
		if (!_eventToObjectives.TryGetValue(eventId, out List<string> objectives))
		{
			objectives = new List<string>();
			_eventToObjectives[eventId] = objectives;
		}
		if (!objectives.Contains(objectiveId)) objectives.Add(objectiveId);
		return true;
	}
	public IReadOnlyList<string> ResolveObjectives(string eventId)
	{
		return !string.IsNullOrWhiteSpace(eventId) && _eventToObjectives.TryGetValue(eventId, out List<string> objectives)
			? objectives.ToArray()
			: System.Array.Empty<string>();
	}
}
