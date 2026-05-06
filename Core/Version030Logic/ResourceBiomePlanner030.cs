using System.Collections.Generic;
using System.Linq;

public sealed class ResourceBiome030
{
	public string Id { get; }
	public IReadOnlyList<string> ResourceItemIds { get; }
	public int RecommendedDanger { get; }
	public ResourceBiome030(string id, IEnumerable<string> resourceItemIds, int recommendedDanger)
	{
		Id = string.IsNullOrWhiteSpace(id) ? "biome" : id;
		ResourceItemIds = (resourceItemIds ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
		RecommendedDanger = System.Math.Max(1, recommendedDanger);
	}
}

public static class ResourceBiomePlanner030
{
	public static ResourceBiome030 PickForDanger(int dangerLevel)
	{
		if (dangerLevel <= 1) return new ResourceBiome030("safe_scraps", new[] { "wooden_plank", "rock", "canned_food" }, 1);
		if (dangerLevel <= 3) return new ResourceBiome030("junk_fields", new[] { "wooden_plank", "rock", "metal", "water_bottle" }, 3);
		return new ResourceBiome030("deep_ruins", new[] { "metal", "reinforced_plate", "generator_part", "ammo_pack" }, 5);
	}
}
