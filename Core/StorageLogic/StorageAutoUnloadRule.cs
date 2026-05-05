public sealed class StorageAutoUnloadRule
{
	public ItemCategory Category { get; }
	public bool ShouldUnload { get; }
	public StorageAutoUnloadRule(ItemCategory category, bool shouldUnload) { Category = category; ShouldUnload = shouldUnload; }
}
