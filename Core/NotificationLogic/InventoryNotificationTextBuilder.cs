public static class InventoryNotificationTextBuilder
{
	public static string BuildPickup(string displayName, int amount, int finalInventoryAmount)
	{
		if (string.IsNullOrWhiteSpace(displayName))
			return string.Empty;

		if (amount <= 0)
			return string.Empty;

		if (finalInventoryAmount < 0)
			return string.Empty;

		return $"+{amount} {displayName} ({finalInventoryAmount})";
	}

	public static string BuildDrop(string displayName, int amount, int finalInventoryAmount)
	{
		if (string.IsNullOrWhiteSpace(displayName))
			return string.Empty;

		if (amount <= 0)
			return string.Empty;

		if (finalInventoryAmount < 0)
			return string.Empty;

		return $"-{amount} {displayName} ({finalInventoryAmount})";
	}
}
