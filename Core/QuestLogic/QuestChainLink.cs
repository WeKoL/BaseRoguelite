public sealed class QuestChainLink
{
	public string QuestId { get; }
	public string UnlocksQuestId { get; }
	public QuestChainLink(string questId, string unlocksQuestId) { QuestId = questId ?? string.Empty; UnlocksQuestId = unlocksQuestId ?? string.Empty; }
}
