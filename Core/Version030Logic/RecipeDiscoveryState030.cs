using System.Collections.Generic;
using System.Linq;

public sealed class RecipeDiscoveryState030
{
	private readonly HashSet<string> _known = new();
	public IReadOnlyCollection<string> KnownRecipeIds => _known;
	public bool Learn(string recipeId) => !string.IsNullOrWhiteSpace(recipeId) && _known.Add(recipeId);
	public bool Knows(string recipeId) => !string.IsNullOrWhiteSpace(recipeId) && _known.Contains(recipeId);
	public IReadOnlyList<CraftRecipe> FilterKnown(IEnumerable<CraftRecipe> recipes)
	{
		return (recipes ?? System.Array.Empty<CraftRecipe>()).Where(r => r != null && Knows(r.Id)).ToList();
	}
}
