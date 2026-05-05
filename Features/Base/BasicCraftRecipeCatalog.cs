using System.Collections.Generic;
public static class BasicCraftRecipeCatalog
{
	public static IReadOnlyList<CraftRecipe> CreateRecipes()=>new List<CraftRecipe>{ new CraftRecipe("craft_simple_medkit","Простая аптечка", new[]{new CraftIngredient("wooden_plank",2),new CraftIngredient("metal",1)}, "simple_medkit",1), new CraftRecipe("craft_reinforced_plate","Усиленная пластина", new[]{new CraftIngredient("rock",2),new CraftIngredient("metal",2)}, "reinforced_plate",1)};
}
