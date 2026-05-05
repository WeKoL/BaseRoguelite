using Xunit;

public sealed class Version30QuestTests
{
	[Fact]
	public void QuestReward_CanBeClaimedOnce()
	{
		QuestDefinition quest = new("q1", "Собери", "", "wood", 2, new[] { new QuestReward("metal", 1) });
		QuestProgressState progress = new();
		StorageState storage = new();
		progress.AddProgress(quest, 2);
		Assert.True(progress.TryClaimReward(quest, storage));
		Assert.False(progress.TryClaimReward(quest, storage));
		Assert.Equal(1, storage.GetTotalAmount("metal"));
	}

	[Fact]
	public void QuestChain_UnlocksNextQuestWhenPreviousCompleted()
	{
		QuestDefinition quest = new("q1", "Q1", "", "wood", 1);
		QuestProgressState progress = new();
		progress.AddProgress(quest, 1);
		var unlocked = QuestChainLogic.GetUnlockedQuestIds(progress, new[] { new QuestChainLink("q1", "q2") });
		Assert.Single(unlocked);
		Assert.Equal("q2", unlocked[0]);
	}

	[Fact]
	public void RecipeUnlockReward_AddsRecipeOnce()
	{
		QuestRecipeUnlockState state = new();
		Assert.True(state.Unlock(new QuestRecipeUnlockReward("medkit")));
		Assert.False(state.Unlock(new QuestRecipeUnlockReward("medkit")));
		Assert.True(state.IsUnlocked("medkit"));
	}
}
