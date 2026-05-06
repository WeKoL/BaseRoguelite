using System;

public sealed class SurvivalNeedsState
{
	public int MaxFood { get; }
	public int Food { get; private set; }
	public int MaxWater { get; }
	public int Water { get; private set; }
	public bool IsHungry => Food <= MaxFood / 4;
	public bool IsThirsty => Water <= MaxWater / 4;
	public bool IsCritical => Food <= 0 || Water <= 0;

	public SurvivalNeedsState(int maxFood = 100, int maxWater = 100)
	{
		MaxFood = Math.Max(1, maxFood);
		MaxWater = Math.Max(1, maxWater);
		Food = MaxFood;
		Water = MaxWater;
	}

	public void Consume(int food, int water)
	{
		Food = Math.Max(0, Food - Math.Max(0, food));
		Water = Math.Max(0, Water - Math.Max(0, water));
	}

	public int Eat(int amount)
	{
		int before = Food;
		Food = Math.Min(MaxFood, Food + Math.Max(0, amount));
		return Food - before;
	}

	public int Drink(int amount)
	{
		int before = Water;
		Water = Math.Min(MaxWater, Water + Math.Max(0, amount));
		return Water - before;
	}

	public float GetFoodRatio()
	{
		return MaxFood <= 0 ? 0f : Math.Clamp(Food / (float)MaxFood, 0f, 1f);
	}

	public float GetWaterRatio()
	{
		return MaxWater <= 0 ? 0f : Math.Clamp(Water / (float)MaxWater, 0f, 1f);
	}
}
