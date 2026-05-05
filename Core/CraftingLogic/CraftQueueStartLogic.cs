public static class CraftQueueStartLogic
{
	public static bool TryStartCraft(StorageState storage, CraftQueueState queue, CraftRecipe recipe, BaseProgressState progress = null)
	{
		if (storage == null || queue == null || recipe == null) return false; if (!CraftingLogic.CanCraft(storage, recipe, progress)) return false; if (!queue.TryEnqueue(recipe)) return false; foreach (CraftIngredient ingredient in recipe.Ingredients) storage.RemoveItem(ingredient.ItemId, ingredient.Amount); return true;
	}
	public static bool ReturnIngredients(StorageState storage, CraftRecipe recipe, int maxStackSize = 99)
	{
		if (storage == null || recipe == null) return false; foreach (CraftIngredient ingredient in recipe.Ingredients) storage.AddItem(ingredient.ItemId, ingredient.Amount, maxStackSize); return true;
	}
}
