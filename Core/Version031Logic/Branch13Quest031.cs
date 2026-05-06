using System;
using System.Collections.Generic;
using System.Linq;

public sealed class QuestMilestone031
{
	public string Id { get; }
	public int RequiredProgress { get; }
	public string RewardId { get; }
	public QuestMilestone031(string id, int requiredProgress, string rewardId)
	{
		Id = id ?? string.Empty;
		RequiredProgress = Math.Max(1, requiredProgress);
		RewardId = rewardId ?? string.Empty;
	}
}

public sealed class QuestMilestonePlanner031
{
	private readonly List<QuestMilestone031> _milestones = new();
	public void Add(QuestMilestone031 milestone)
	{
		if (milestone != null) _milestones.Add(milestone);
	}
	public IReadOnlyList<QuestMilestone031> GetCompleted(int progress)
	{
		return _milestones.Where(x => progress >= x.RequiredProgress).OrderBy(x => x.RequiredProgress).ToList();
	}
}

public sealed class TutorialFlow031
{
	private readonly List<string> _steps = new();
	private int _index;
	public string Current => _index >= 0 && _index < _steps.Count ? _steps[_index] : string.Empty;
	public bool IsCompleted => _steps.Count > 0 && _index >= _steps.Count;
	public TutorialFlow031(IEnumerable<string> steps)
	{
		_steps.AddRange((steps ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)));
	}
	public void CompleteCurrent()
	{
		if (_index < _steps.Count) _index++;
	}
}
