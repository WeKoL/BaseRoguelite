using System.Collections.Generic;
public static class QuestChainLogic
{
	public static IReadOnlyList<string> GetUnlockedQuestIds(QuestProgressState progress, IEnumerable<QuestChainLink> links)
	{
		List<string> result = new(); if (progress == null || links == null) return result; foreach (QuestChainLink link in links) if (link != null && progress.IsCompleted(link.QuestId) && !string.IsNullOrWhiteSpace(link.UnlocksQuestId)) result.Add(link.UnlocksQuestId); return result;
	}
}
