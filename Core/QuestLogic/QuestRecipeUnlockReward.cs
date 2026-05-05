public sealed class QuestRecipeUnlockReward
{
	public string RecipeId { get; }
	public QuestRecipeUnlockReward(string recipeId) { RecipeId = recipeId ?? string.Empty; }
}
