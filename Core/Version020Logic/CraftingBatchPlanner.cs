using System;
using System.Collections.Generic;

public sealed class CraftingMissingIngredient
{
	public string ItemId { get; }
	public int RequiredAmount { get; }
	public int AvailableAmount { get; }
	public int MissingAmount => Math.Max(0, RequiredAmount - AvailableAmount);

	public CraftingMissingIngredient(string itemId, int requiredAmount, int availableAmount)
	{
		ItemId = itemId ?? string.Empty;
		RequiredAmount = Math.Max(0, requiredAmount);
		AvailableAmount = Math.Max(0, availableAmount);
	}
}

public sealed class CraftingBatchPlan
{
	public int MaxBatches { get; }
	public IReadOnlyList<CraftingMissingIngredient> MissingIngredients { get; }
	public bool CanCraftAtLeastOne => MaxBatches > 0 && MissingIngredients.Count == 0;

	public CraftingBatchPlan(int maxBatches, IReadOnlyList<CraftingMissingIngredient> missingIngredients)
	{
		MaxBatches = Math.Max(0, maxBatches);
		MissingIngredients = missingIngredients ?? Array.Empty<CraftingMissingIngredient>();
	}
}

public static class CraftingBatchPlanner
{
	public static CraftingBatchPlan BuildPlan(StorageState storage, CraftRecipe recipe, int desiredBatches)
	{
		if (storage == null || recipe == null || recipe.Ingredients.Count == 0)
			return new CraftingBatchPlan(0, Array.Empty<CraftingMissingIngredient>());

		int maxByStorage = int.MaxValue;
		var missing = new List<CraftingMissingIngredient>();
		int batches = Math.Max(1, desiredBatches);

		foreach (CraftIngredient ingredient in recipe.Ingredients)
		{
			int available = storage.GetTotalAmount(ingredient.ItemId);
			maxByStorage = Math.Min(maxByStorage, ingredient.Amount <= 0 ? 0 : available / ingredient.Amount);

			int requiredForDesired = ingredient.Amount * batches;
			if (available < requiredForDesired)
				missing.Add(new CraftingMissingIngredient(ingredient.ItemId, requiredForDesired, available));
		}

		return new CraftingBatchPlan(maxByStorage == int.MaxValue ? 0 : maxByStorage, missing);
	}
}
