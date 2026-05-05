public sealed class CraftQueueItem
{
	public CraftRecipe Recipe { get; }
	public int RemainingSeconds { get; private set; }
	public bool IsCompleted => RemainingSeconds <= 0;
	public CraftQueueItem(CraftRecipe recipe) { Recipe = recipe; RemainingSeconds = recipe == null ? 0 : System.Math.Max(1, recipe.CraftTimeSeconds); }
	public bool Tick(int seconds) { if (seconds <= 0 || IsCompleted) return IsCompleted; RemainingSeconds = System.Math.Max(0, RemainingSeconds - seconds); return IsCompleted; }
}
