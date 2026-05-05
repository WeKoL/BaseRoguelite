using System.Collections.Generic;
public sealed class QuestRecipeUnlockState
{
	private readonly HashSet<string> _unlockedRecipeIds = new();
	public IReadOnlyCollection<string> UnlockedRecipeIds => _unlockedRecipeIds;
	public bool IsUnlocked(string recipeId) => !string.IsNullOrWhiteSpace(recipeId) && _unlockedRecipeIds.Contains(recipeId);
	public bool Unlock(QuestRecipeUnlockReward reward) { if (reward == null || string.IsNullOrWhiteSpace(reward.RecipeId)) return false; return _unlockedRecipeIds.Add(reward.RecipeId); }
}
