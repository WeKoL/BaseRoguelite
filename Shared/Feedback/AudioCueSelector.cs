public static class AudioCueSelector
{
	public static AudioCueKind SelectForFeedback(FeedbackEvent feedback)
	{
		if (feedback == null) return AudioCueKind.None; string lower = (feedback.Text ?? string.Empty).ToLowerInvariant();
		if (lower.Contains("подобр") || lower.Contains("pickup")) return AudioCueKind.Pickup; if (lower.Contains("крафт") || lower.Contains("craft")) return AudioCueKind.Craft; if (lower.Contains("урон") || lower.Contains("hit") || lower.Contains("damage")) return AudioCueKind.Hit; if (lower.Contains("квест") || lower.Contains("quest")) return AudioCueKind.QuestComplete; if (lower.Contains("ошиб") || lower.Contains("fail") || lower.Contains("error")) return AudioCueKind.Error; return AudioCueKind.None;
	}
}
