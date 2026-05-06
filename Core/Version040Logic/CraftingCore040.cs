using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CraftIngredient040
{
	public string ItemId { get; }
	public int Amount { get; }
	public CraftIngredient040(string itemId, int amount) { ItemId = itemId ?? string.Empty; Amount = Math.Max(1, amount); }
}

public sealed class CraftRecipe040
{
	private readonly List<CraftIngredient040> _ingredients = new();
	public string Id { get; }
	public string Name { get; }
	public IReadOnlyList<CraftIngredient040> Ingredients => _ingredients;
	public string OutputItemId { get; }
	public int OutputAmount { get; }
	public BaseFacility040 RequiredFacility { get; }
	public int RequiredFacilityLevel { get; }
	public int Seconds { get; }

	public CraftRecipe040(string id, string name, IEnumerable<CraftIngredient040> ingredients, string outputItemId, int outputAmount, BaseFacility040 requiredFacility = BaseFacility040.None, int requiredFacilityLevel = 0, int seconds = 0)
	{
		Id = id ?? string.Empty;
		Name = string.IsNullOrWhiteSpace(name) ? Id : name;
		if (ingredients != null) _ingredients.AddRange(ingredients);
		OutputItemId = outputItemId ?? string.Empty;
		OutputAmount = Math.Max(1, outputAmount);
		RequiredFacility = requiredFacility;
		RequiredFacilityLevel = Math.Max(0, requiredFacilityLevel);
		Seconds = Math.Max(0, seconds);
	}
}

public sealed class RecipeBook040
{
	private readonly Dictionary<string, CraftRecipe040> _recipes = new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _unlocked = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyCollection<CraftRecipe040> Recipes => _recipes.Values;
	public void Add(CraftRecipe040 recipe, bool unlocked = true) { if (recipe != null) { _recipes[recipe.Id] = recipe; if (unlocked) _unlocked.Add(recipe.Id); } }
	public void Unlock(string recipeId) { if (_recipes.ContainsKey(recipeId ?? string.Empty)) _unlocked.Add(recipeId); }
	public bool IsUnlocked(string recipeId) => _unlocked.Contains(recipeId ?? string.Empty);
	public CraftRecipe040 Get(string recipeId) => _recipes[recipeId];

	public static RecipeBook040 CreateDefault()
	{
		RecipeBook040 book = new();
		book.Add(new CraftRecipe040("medkit", "Аптечка", new[] { new CraftIngredient040("cloth", 2), new CraftIngredient040("herb", 1) }, "medkit", 1, BaseFacility040.Medbay, 1, 10));
		book.Add(new CraftRecipe040("helmet", "Шлем", new[] { new CraftIngredient040("metal", 5), new CraftIngredient040("cloth", 2) }, "helmet", 1, BaseFacility040.Workbench, 1, 30));
		book.Add(new CraftRecipe040("armor", "Броня", new[] { new CraftIngredient040("metal", 8), new CraftIngredient040("cloth", 4) }, "armor", 1, BaseFacility040.Workbench, 2, 60));
		book.Add(new CraftRecipe040("ammo", "Патроны", new[] { new CraftIngredient040("metal", 2) }, "ammo_basic", 10, BaseFacility040.Workbench, 2, 20));
		book.Add(new CraftRecipe040("water_filter", "Фильтр воды", new[] { new CraftIngredient040("metal", 3), new CraftIngredient040("cloth", 2) }, "water", 3, BaseFacility040.Generator, 1, 40));
		return book;
	}
}

public sealed class CraftingCore040
{
	private readonly RecipeBook040 _book;
	private readonly BaseCore040 _base;

	public CraftingCore040(RecipeBook040 book, BaseCore040 baseCore)
	{
		_book = book ?? throw new ArgumentNullException(nameof(book));
		_base = baseCore ?? throw new ArgumentNullException(nameof(baseCore));
	}

	public CraftPreview040 Preview(string recipeId, StorageCore040 storage)
	{
		if (!_book.IsUnlocked(recipeId)) return CraftPreview040.Blocked(recipeId, "recipe_locked");
		CraftRecipe040 recipe = _book.Get(recipeId);
		if (!_base.HasFacility(recipe.RequiredFacility, recipe.RequiredFacilityLevel)) return CraftPreview040.Blocked(recipeId, "station_required");
		var missing = recipe.Ingredients.Where(x => storage.Get(x.ItemId) < x.Amount).Select(x => new CraftIngredient040(x.ItemId, x.Amount - storage.Get(x.ItemId))).ToArray();
		return missing.Length == 0 ? CraftPreview040.Ready(recipeId, recipe.OutputItemId, recipe.OutputAmount) : CraftPreview040.Missing(recipeId, missing);
	}

	public CraftPreview040 CraftNow(string recipeId, StorageCore040 storage)
	{
		CraftPreview040 preview = Preview(recipeId, storage);
		if (!preview.CanCraft) return preview;
		CraftRecipe040 recipe = _book.Get(recipeId);
		foreach (CraftIngredient040 ingredient in recipe.Ingredients)
			storage.Remove(ingredient.ItemId, ingredient.Amount, allowReserved: true);
		storage.Add(recipe.OutputItemId, recipe.OutputAmount);
		return preview;
	}
}

public sealed class CraftPreview040
{
	public string RecipeId { get; }
	public bool CanCraft { get; }
	public string Blocker { get; }
	public IReadOnlyList<CraftIngredient040> MissingIngredients { get; }
	public string OutputItemId { get; }
	public int OutputAmount { get; }

	private CraftPreview040(string recipeId, bool canCraft, string blocker, IReadOnlyList<CraftIngredient040> missing, string outputItemId, int outputAmount)
	{
		RecipeId = recipeId ?? string.Empty;
		CanCraft = canCraft;
		Blocker = blocker ?? string.Empty;
		MissingIngredients = missing ?? Array.Empty<CraftIngredient040>();
		OutputItemId = outputItemId ?? string.Empty;
		OutputAmount = outputAmount;
	}

	public static CraftPreview040 Ready(string recipeId, string outputItemId, int outputAmount) => new(recipeId, true, string.Empty, Array.Empty<CraftIngredient040>(), outputItemId, outputAmount);
	public static CraftPreview040 Missing(string recipeId, IReadOnlyList<CraftIngredient040> missing) => new(recipeId, false, "missing_resources", missing, string.Empty, 0);
	public static CraftPreview040 Blocked(string recipeId, string blocker) => new(recipeId, false, blocker, Array.Empty<CraftIngredient040>(), string.Empty, 0);
}

public sealed class CraftQueueCore040
{
	private readonly Queue<(string RecipeId, int RemainingSeconds)> _queue = new();
	public int Count => _queue.Count;
	public void Enqueue(string recipeId, int seconds) { if (!string.IsNullOrWhiteSpace(recipeId)) _queue.Enqueue((recipeId, Math.Max(0, seconds))); }
	public IReadOnlyList<string> Tick(int seconds)
	{
		if (_queue.Count == 0) return Array.Empty<string>();
		int budget = Math.Max(0, seconds);
		List<string> completed = new();
		while (_queue.Count > 0 && budget >= 0)
		{
			var item = _queue.Dequeue();
			if (budget >= item.RemainingSeconds)
			{
				budget -= item.RemainingSeconds;
				completed.Add(item.RecipeId);
				continue;
			}
			_queue.Enqueue((item.RecipeId, item.RemainingSeconds - budget));
			break;
		}
		return completed;
	}
}
