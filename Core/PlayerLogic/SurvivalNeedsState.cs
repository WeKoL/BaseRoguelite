public sealed class SurvivalNeedsState
{
	public int MaxFood { get; }
	public int Food { get; private set; }
	public int MaxWater { get; }
	public int Water { get; private set; }
	public bool IsHungry => Food <= MaxFood / 4;
	public bool IsThirsty => Water <= MaxWater / 4;
	public SurvivalNeedsState(int maxFood = 100, int maxWater = 100) { MaxFood = System.Math.Max(1, maxFood); MaxWater = System.Math.Max(1, maxWater); Food = MaxFood; Water = MaxWater; }
	public void Consume(int food, int water) { Food = System.Math.Max(0, Food - System.Math.Max(0, food)); Water = System.Math.Max(0, Water - System.Math.Max(0, water)); }
	public int Eat(int amount) { int before = Food; Food = System.Math.Min(MaxFood, Food + System.Math.Max(0, amount)); return Food - before; }
	public int Drink(int amount) { int before = Water; Water = System.Math.Min(MaxWater, Water + System.Math.Max(0, amount)); return Water - before; }
}
