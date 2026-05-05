public static class StorageCompactionLogic
{
	public static int Compact(StorageState storage)
	{
		if (storage == null) return 0;
		int merged = 0;
		for (int i = storage.Entries.Count - 1; i >= 0; i--)
		{
			StorageEntryState source = storage.Entries[i];
			if (source == null || source.Amount <= 0) continue;
			for (int j = 0; j < i; j++)
			{
				StorageEntryState target = storage.Entries[j];
				if (target == null || target.ItemId != source.ItemId) continue;
				int moved = target.AddAmount(source.Amount);
				source.RemoveAmount(moved);
				merged += moved;
				if (source.Amount <= 0) break;
			}
		}
		storage.RemoveEmptyEntries();
		return merged;
	}
}
