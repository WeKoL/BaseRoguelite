using System;
using System.Collections.Generic;

public sealed class BaseUpgradePlanEntry
{
	public BaseUpgradeDefinition Upgrade { get; }
	public bool CanApply { get; }
	public IReadOnlyList<CraftingMissingIngredient> MissingIngredients { get; }

	public BaseUpgradePlanEntry(BaseUpgradeDefinition upgrade, bool canApply, IReadOnlyList<CraftingMissingIngredient> missingIngredients)
	{
		Upgrade = upgrade;
		CanApply = canApply;
		MissingIngredients = missingIngredients ?? Array.Empty<CraftingMissingIngredient>();
	}
}

public static class BaseUpgradePlanner
{
	public static IReadOnlyList<BaseUpgradePlanEntry> BuildPlan(StorageState storage, IEnumerable<BaseUpgradeDefinition> upgrades)
	{
		var result = new List<BaseUpgradePlanEntry>();
		if (storage == null || upgrades == null) return result;

		foreach (BaseUpgradeDefinition upgrade in upgrades)
		{
			if (upgrade == null) continue;
			var missing = new List<CraftingMissingIngredient>();
			foreach (CraftIngredient ingredient in upgrade.Cost)
			{
				int available = storage.GetTotalAmount(ingredient.ItemId);
				if (available < ingredient.Amount)
					missing.Add(new CraftingMissingIngredient(ingredient.ItemId, ingredient.Amount, available));
			}
			result.Add(new BaseUpgradePlanEntry(upgrade, missing.Count == 0, missing));
		}

		return result;
	}
}
