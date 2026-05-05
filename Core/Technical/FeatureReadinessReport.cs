using System.Collections.Generic;
public sealed class FeatureReadinessReport
{
	private readonly List<FeatureReadinessItem> _items = new();
	public IReadOnlyList<FeatureReadinessItem> Items => _items;
	public int TotalWeight { get; private set; }
	public int DoneWeight { get; private set; }
	public int MissingCount { get; private set; }
	public float CompletionRatio => TotalWeight <= 0 ? 0f : DoneWeight / (float)TotalWeight;
	public bool IsReady => TotalWeight > 0 && MissingCount == 0;
	public void Add(FeatureReadinessItem item) { if (item == null) return; _items.Add(item); TotalWeight += item.Weight; if (item.IsDone) DoneWeight += item.Weight; else MissingCount++; }
}
