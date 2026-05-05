using System;

public static class SafeZoneUnloadEntryNotificationTextBuilder
{
	public static string Build(
		SafeZoneUnloadEntry entry,
		Func<string, string> displayNameResolver = null)
	{
		if (entry == null || entry.MovedAmount <= 0)
			return string.Empty;

		string displayName = ResolveDisplayName(entry.ItemId, displayNameResolver);
		return $"+{entry.MovedAmount} {displayName} ({entry.FinalStorageAmount})";
	}

	private static string ResolveDisplayName(string itemId, Func<string, string> displayNameResolver)
	{
		string resolvedName = displayNameResolver?.Invoke(itemId);
		return string.IsNullOrWhiteSpace(resolvedName)
			? itemId ?? string.Empty
			: resolvedName;
	}
}
