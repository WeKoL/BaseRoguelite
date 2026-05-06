using System.Collections.Generic;

public static class BasicBaseUpgradeCatalog
{
	public static IReadOnlyList<BaseUpgradeDefinition> CreateUpgrades() => new List<BaseUpgradeDefinition>
	{
		new BaseUpgradeDefinition(
			"storage_box",
			"Усилить хранилище",
			new[] { new CraftIngredient("wooden_plank", 4), new CraftIngredient("metal", 2) }),

		new BaseUpgradeDefinition(
			"workbench",
			"Собрать верстак",
			new[] { new CraftIngredient("wooden_plank", 6), new CraftIngredient("rock", 3), new CraftIngredient("metal", 2) }),

		new BaseUpgradeDefinition(
			"medical_station",
			"Собрать медпункт",
			new[] { new CraftIngredient("wooden_plank", 2), new CraftIngredient("metal", 3), new CraftIngredient("simple_medkit", 1) }),

		new BaseUpgradeDefinition(
			"generator",
			"Запустить генератор",
			new[] { new CraftIngredient("metal", 4), new CraftIngredient("generator_part", 1), new CraftIngredient("reinforced_plate", 1) }),

		new BaseUpgradeDefinition(
			"radio_tower",
			"Поставить радиомачту",
			new[] { new CraftIngredient("metal", 5), new CraftIngredient("generator_part", 1), new CraftIngredient("tool_kit", 1) }),

		new BaseUpgradeDefinition(
			"defensive_walls",
			"Укрепить стены",
			new[] { new CraftIngredient("wooden_plank", 8), new CraftIngredient("metal", 4), new CraftIngredient("reinforced_plate", 2) })
	};
}
