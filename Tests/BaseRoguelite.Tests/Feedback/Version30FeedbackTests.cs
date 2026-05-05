using Xunit;

public sealed class Version30FeedbackTests
{
	[Fact]
	public void AudioCueSelector_RecognizesPickupAndCraftText()
	{
		Assert.Equal(AudioCueKind.Pickup, AudioCueSelector.SelectForFeedback(new FeedbackEvent("Подобрано: дерево")));
		Assert.Equal(AudioCueKind.Craft, AudioCueSelector.SelectForFeedback(new FeedbackEvent("Скрафчено: аптечка")));
	}

	[Fact]
	public void DamageFeedbackPlanner_MarksCriticalDamage()
	{
		DamageFeedbackPlan plan = DamageFeedbackPlanner.Build(30, 20);
		Assert.True(plan.ShowCriticalText);
		Assert.True(plan.ShakeStrength > 0);
	}

	[Fact]
	public void FeedbackQueue_PreservesOrder()
	{
		FeedbackQueue queue = new();
		queue.Enqueue(new FeedbackEvent("first"));
		queue.Enqueue(new FeedbackEvent("second"));
		Assert.Equal("first", queue.Dequeue().Text);
		Assert.Equal("second", queue.Dequeue().Text);
	}
}
