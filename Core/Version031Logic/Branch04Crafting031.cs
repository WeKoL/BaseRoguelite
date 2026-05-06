using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CraftPreview031
{
	public string RecipeId { get; }
	public IReadOnlyList<string> MissingItems { get; }
	public bool CanCraft => MissingItems.Count == 0;
	public string Message => CanCraft ? "can_craft" : "missing:" + string.Join(",", MissingItems);

	public CraftPreview031(string recipeId, IEnumerable<string> missingItems)
	{
		RecipeId = recipeId ?? string.Empty;
		MissingItems = (missingItems ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
	}
}

public static class CraftPreviewBuilder031
{
	public static CraftPreview031 Build(string recipeId, IEnumerable<(string ItemId, int Required, int Available)> ingredients)
	{
		List<string> missing = new();
		foreach (var ingredient in ingredients ?? Array.Empty<(string ItemId, int Required, int Available)>())
		{
			if (ingredient.Available < ingredient.Required)
				missing.Add(ingredient.ItemId);
		}
		return new CraftPreview031(recipeId, missing);
	}
}

public sealed class RecipeDependencyGraph031
{
	private readonly Dictionary<string, List<string>> _requires = new();

	public void AddRequirement(string recipeId, string requiredRecipeId)
	{
		if (string.IsNullOrWhiteSpace(recipeId) || string.IsNullOrWhiteSpace(requiredRecipeId))
			return;
		if (!_requires.ContainsKey(recipeId))
			_requires[recipeId] = new List<string>();
		if (!_requires[recipeId].Contains(requiredRecipeId))
			_requires[recipeId].Add(requiredRecipeId);
	}

	public bool IsUnlocked(string recipeId, IEnumerable<string> completedRecipes)
	{
		HashSet<string> completed = new(completedRecipes ?? Array.Empty<string>());
		return !_requires.TryGetValue(recipeId, out var required) || required.All(completed.Contains);
	}

	public IReadOnlyList<string> GetMissingDependencies(string recipeId, IEnumerable<string> completedRecipes)
	{
		HashSet<string> completed = new(completedRecipes ?? Array.Empty<string>());
		return !_requires.TryGetValue(recipeId, out var required) ? Array.Empty<string>() : required.Where(x => !completed.Contains(x)).ToList();
	}
}
