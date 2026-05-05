using System;

public sealed class InventoryItemDefinition
{
	public string Id { get; }
	public int MaxStackSize { get; }
	public float Weight { get; }

	public InventoryItemDefinition(string id, int maxStackSize, float weight)
	{
		Id = id ?? string.Empty;
		MaxStackSize = Math.Max(1, maxStackSize);
		Weight = Math.Max(0f, weight);
	}

	public bool IsValid()
	{
		return !string.IsNullOrWhiteSpace(Id) && MaxStackSize > 0;
	}
}
