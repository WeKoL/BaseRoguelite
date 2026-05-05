using System.Collections.Generic;
public sealed class QuestProgressState
{
	private readonly Dictionary<string, int> _progress = new();
	private readonly HashSet<string> _completed = new();
	private readonly HashSet<string> _claimed = new();
	public int GetProgress(string id) => !string.IsNullOrWhiteSpace(id) && _progress.TryGetValue(id, out int value) ? value : 0;
	public bool IsCompleted(string id) => !string.IsNullOrWhiteSpace(id) && _completed.Contains(id);
	public bool IsRewardClaimed(string id) => !string.IsNullOrWhiteSpace(id) && _claimed.Contains(id);
	public void AddProgress(QuestDefinition quest, int amount) { if (quest == null || amount <= 0 || IsCompleted(quest.Id)) return; int current = GetProgress(quest.Id) + amount; _progress[quest.Id] = current; if (current >= quest.TargetAmount) _completed.Add(quest.Id); }
	public bool TryClaimReward(QuestDefinition quest, StorageState storage, int rewardMaxStackSize = 99)
	{
		if (quest == null || storage == null || !IsCompleted(quest.Id) || IsRewardClaimed(quest.Id)) return false;
		foreach (QuestReward reward in quest.Rewards) if (!storage.CanFit(reward.ItemId, reward.Amount, rewardMaxStackSize)) return false;
		foreach (QuestReward reward in quest.Rewards) storage.AddItem(reward.ItemId, reward.Amount, rewardMaxStackSize);
		_claimed.Add(quest.Id); return true;
	}
}
