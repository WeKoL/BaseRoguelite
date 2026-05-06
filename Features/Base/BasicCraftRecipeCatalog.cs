using System.Collections.Generic;

public static class BasicCraftRecipeCatalog
{
	public static IReadOnlyList<CraftRecipe> CreateRecipes() => new List<CraftRecipe>
	{
		new CraftRecipe(
			"craft_simple_medkit",
			"Простая аптечка",
			new[] { new CraftIngredient("wooden_plank", 2), new CraftIngredient("metal", 1) },
			"simple_medkit",
			1),

		new CraftRecipe(
			"craft_reinforced_plate",
			"Усиленная пластина",
			new[] { new CraftIngredient("rock", 2), new CraftIngredient("metal", 2) },
			"reinforced_plate",
			1),

		new CraftRecipe(
			"craft_repair_kit",
			"Ремонтный набор",
			new[] { new CraftIngredient("wooden_plank", 1), new CraftIngredient("metal", 2), new CraftIngredient("reinforced_plate", 1) },
			"repair_kit",
			1,
			"workbench",
			1),

		new CraftRecipe(
			"craft_generator_part",
			"Деталь генератора",
			new[] { new CraftIngredient("metal", 4), new CraftIngredient("reinforced_plate", 1) },
			"generator_part",
			1,
			"workbench",
			1),

		new CraftRecipe(
			"craft_field_ration",
			"Полевой рацион",
			new[] { new CraftIngredient("wooden_plank", 1), new CraftIngredient("rock", 1) },
			"canned_food",
			1,
			"medical_station",
			1),

		new CraftRecipe(
			"craft_water_filter",
			"Фильтр воды",
			new[] { new CraftIngredient("rock", 2), new CraftIngredient("metal", 1) },
			"water_filter",
			1,
			"workbench",
			1,
			8),

		new CraftRecipe(
			"craft_tool_kit",
			"Набор инструментов",
			new[] { new CraftIngredient("metal", 3), new CraftIngredient("reinforced_plate", 1) },
			"tool_kit",
			1,
			"workbench",
			2,
			12),

		new CraftRecipe(
			"craft_ammo_pack",
			"Пачка боеприпасов",
			new[] { new CraftIngredient("metal", 2), new CraftIngredient("rock", 1) },
			"ammo_pack",
			1,
			"workbench",
			1,
			6),

		new CraftRecipe(
			"craft_camp_beacon",
			"Маяк лагеря",
			new[] { new CraftIngredient("wooden_plank", 4), new CraftIngredient("metal", 4), new CraftIngredient("generator_part", 1) },
			"camp_beacon",
			1,
			"generator",
			1,
			20)
	};
}
