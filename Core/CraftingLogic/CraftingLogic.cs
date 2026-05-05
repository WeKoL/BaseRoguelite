public static class CraftingLogic
{
	public static bool CanCraft(StorageState storage, CraftRecipe recipe) => CanCraft(storage, recipe, null);

	public static bool CanCraft(StorageState storage, CraftRecipe recipe, BaseProgressState progress)
	{
		if (storage == null || recipe == null) return false;
		if (!HasRequiredStation(recipe, progress)) return false;
		foreach (CraftIngredient ingredient in recipe.Ingredients)
			if (storage.GetTotalAmount(ingredient.ItemId) < ingredient.Amount) return false;
		return true;
	}

	public static bool HasRequiredStation(CraftRecipe recipe, BaseProgressState progress)
	{
		if (recipe == null) return false;
		if (!recipe.RequiresStation) return true;
		if (progress == null) return false;
		return progress.GetUpgradeLevel(recipe.RequiredStationId) >= recipe.RequiredStationLevel;
	}

	public static CraftResult TryCraft(StorageState storage, CraftRecipe recipe, InventoryItemDefinition output, int outputAmount) => TryCraft(storage, recipe, output, outputAmount, null);

	public static CraftResult TryCraft(StorageState storage, CraftRecipe recipe, InventoryItemDefinition output, int outputAmount, BaseProgressState progress)
	{
		if (storage == null || recipe == null || output == null || !output.IsValid() || outputAmount <= 0) return CraftResult.Fail("invalid");
		if (!HasRequiredStation(recipe, progress)) return CraftResult.Fail("station_required");
		if (!CanCraft(storage, recipe, progress)) return CraftResult.Fail("not_enough_resources");
		if (!storage.CanFit(output.Id, outputAmount, output.MaxStackSize)) return CraftResult.Fail("storage_full");
		foreach (CraftIngredient ingredient in recipe.Ingredients) storage.RemoveItem(ingredient.ItemId, ingredient.Amount);
		int added = storage.TryAddItem(output.Id, outputAmount, output.MaxStackSize);
		return added >= outputAmount ? CraftResult.Success() : CraftResult.Fail("storage_full");
	}

	public static int EstimateCraftBatches(StorageState storage, CraftRecipe recipe)
	{
		if (storage == null || recipe == null || recipe.Ingredients.Count == 0) return 0;
		int batches = int.MaxValue;
		foreach (CraftIngredient ingredient in recipe.Ingredients)
			batches = System.Math.Min(batches, storage.GetTotalAmount(ingredient.ItemId) / ingredient.Amount);
		return batches == int.MaxValue ? 0 : batches;
	}
}
