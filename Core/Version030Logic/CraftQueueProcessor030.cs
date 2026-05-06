using System.Collections.Generic;

public sealed class CompletedCraft030
{
	public string OutputItemId { get; }
	public int OutputAmount { get; }
	public CompletedCraft030(string outputItemId, int outputAmount)
	{
		OutputItemId = outputItemId ?? string.Empty;
		OutputAmount = System.Math.Max(0, outputAmount);
	}
}

public sealed class CraftQueueProcessor030
{
	private readonly List<CraftQueueItem> _queue = new();
	public IReadOnlyList<CraftQueueItem> Queue => _queue;

	public bool Enqueue(CraftRecipe recipe, int batches = 1)
	{
		if (recipe == null || batches <= 0) return false;
		for (int i = 0; i < batches; i++)
			_queue.Add(new CraftQueueItem(recipe));
		return true;
	}

	public IReadOnlyList<CompletedCraft030> Tick(float seconds, IEnumerable<CraftRecipe> knownRecipes)
	{
		List<CompletedCraft030> completed = new();
		if (seconds <= 0f || _queue.Count == 0) return completed;
		for (int i = _queue.Count - 1; i >= 0; i--)
		{
			CraftQueueItem item = _queue[i];
			item.Tick((int)System.Math.Ceiling(seconds));
			if (!item.IsCompleted) continue;
			_queue.RemoveAt(i);
			if (item.Recipe != null) completed.Add(new CompletedCraft030(item.Recipe.OutputItemId, item.Recipe.OutputAmount));
		}
		completed.Reverse();
		return completed;
	}
}
