using System;

public sealed class SaveDiff030
{
	public int InventoryDelta { get; }
	public int StorageDelta { get; }
	public int QuestDelta { get; }
	public bool HasGameplayProgress => InventoryDelta != 0 || StorageDelta != 0 || QuestDelta != 0;
	public SaveDiff030(int inventoryDelta, int storageDelta, int questDelta)
	{
		InventoryDelta = inventoryDelta;
		StorageDelta = storageDelta;
		QuestDelta = questDelta;
	}
}

public static class SaveDiffBuilder030
{
	public static SaveDiff030 Compare(SaveGameData before, SaveGameData after)
	{
		if (before == null || after == null) return new SaveDiff030(0, 0, 0);
		return new SaveDiff030(
			(after.InventoryItems?.Count ?? 0) - (before.InventoryItems?.Count ?? 0),
			(after.StorageItems?.Count ?? 0) - (before.StorageItems?.Count ?? 0),
			(after.QuestProgress?.Count ?? 0) - (before.QuestProgress?.Count ?? 0));
	}
}
