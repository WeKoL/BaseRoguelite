using System.Collections.Generic;
public sealed class QuestDefinition
{
	private readonly List<QuestReward> _rewards = new();
	public string Id { get; }
	public string Title { get; }
	public string Description { get; }
	public string TargetItemId { get; }
	public int TargetAmount { get; }
	public IReadOnlyList<QuestReward> Rewards => _rewards;
	public QuestDefinition(string id, string title, string description, string targetItemId, int targetAmount) : this(id, title, description, targetItemId, targetAmount, null) { }
	public QuestDefinition(string id, string title, string description, string targetItemId, int targetAmount, IEnumerable<QuestReward> rewards)
	{
		Id = id ?? string.Empty; Title = string.IsNullOrWhiteSpace(title) ? Id : title; Description = description ?? string.Empty; TargetItemId = targetItemId ?? string.Empty; TargetAmount = targetAmount <= 0 ? 1 : targetAmount; if (rewards != null) _rewards.AddRange(rewards);
	}
}
