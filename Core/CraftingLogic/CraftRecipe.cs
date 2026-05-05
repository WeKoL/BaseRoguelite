using System.Collections.Generic;

public sealed class CraftRecipe
{
	private readonly List<CraftIngredient> _ingredients = new();
	public string Id { get; }
	public string DisplayName { get; }
	public IReadOnlyList<CraftIngredient> Ingredients => _ingredients;
	public string OutputItemId { get; }
	public int OutputAmount { get; }
	public string RequiredStationId { get; }
	public int RequiredStationLevel { get; }
	public int CraftTimeSeconds { get; }
	public bool RequiresStation => !string.IsNullOrWhiteSpace(RequiredStationId) && RequiredStationLevel > 0;

	public CraftRecipe(string id, string displayName, IEnumerable<CraftIngredient> ingredients, string outputItemId, int outputAmount)
		: this(id, displayName, ingredients, outputItemId, outputAmount, string.Empty, 0, 0) { }

	public CraftRecipe(string id, string displayName, IEnumerable<CraftIngredient> ingredients, string outputItemId, int outputAmount, string requiredStationId, int requiredStationLevel, int craftTimeSeconds = 0)
	{
		Id = id ?? string.Empty;
		DisplayName = string.IsNullOrWhiteSpace(displayName) ? Id : displayName;
		if (ingredients != null) _ingredients.AddRange(ingredients);
		OutputItemId = outputItemId ?? string.Empty;
		OutputAmount = System.Math.Max(1, outputAmount);
		RequiredStationId = requiredStationId ?? string.Empty;
		RequiredStationLevel = System.Math.Max(0, requiredStationLevel);
		CraftTimeSeconds = System.Math.Max(0, craftTimeSeconds);
	}
}
