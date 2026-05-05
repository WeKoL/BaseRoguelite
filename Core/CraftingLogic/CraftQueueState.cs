using System.Collections.Generic;
public sealed class CraftQueueState
{
	private readonly Queue<CraftQueueItem> _items = new();
	public int MaxQueueSize { get; }
	public int Count => _items.Count;
	public CraftQueueItem Current => _items.Count <= 0 ? null : _items.Peek();
	public CraftQueueState(int maxQueueSize) { MaxQueueSize = maxQueueSize <= 0 ? 1 : maxQueueSize; }
	public bool TryEnqueue(CraftRecipe recipe) { if (recipe == null || _items.Count >= MaxQueueSize) return false; _items.Enqueue(new CraftQueueItem(recipe)); return true; }
	public CraftQueueItem Tick(int seconds) { if (_items.Count <= 0 || seconds <= 0) return null; CraftQueueItem current = _items.Peek(); if (!current.Tick(seconds)) return null; return _items.Dequeue(); }
	public CraftRecipe CancelCurrent() { if (_items.Count <= 0) return null; return _items.Dequeue().Recipe; }
}
