public static class QuestLogTextBuilder
{
	public static string BuildProgressLine(QuestDefinition quest, QuestProgressState state)
	{
		if (quest == null || state == null) return string.Empty; int progress = System.Math.Min(state.GetProgress(quest.Id), quest.TargetAmount); string suffix = state.IsCompleted(quest.Id) ? " — выполнено" : string.Empty; return $"{quest.Title}: {progress}/{quest.TargetAmount}{suffix}";
	}
}
