public sealed class QuestReward
{
	public string ItemId { get; }
	public int Amount { get; }
	public QuestReward(string itemId, int amount) { ItemId = itemId ?? string.Empty; Amount = amount <= 0 ? 1 : amount; }
}
