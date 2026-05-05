using System.Collections.Generic;
public static class StorageSearchLogic
{
	public static IReadOnlyList<StorageEntryState> SearchByItemId(StorageState storage, string query)
	{
		List<StorageEntryState> result = new(); if (storage == null || string.IsNullOrWhiteSpace(query)) return result;
		string normalized = query.ToLowerInvariant(); foreach (StorageEntryState e in storage.Entries) if (e.ItemId != null && e.ItemId.ToLowerInvariant().Contains(normalized)) result.Add(e); return result;
	}
}
