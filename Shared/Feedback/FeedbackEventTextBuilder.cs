public static class FeedbackEventTextBuilder
{
	public static string BuildPickupText(string itemName, int amount) { return $"Подобрано: {itemName} x{System.Math.Max(1, amount)}"; }
	public static string BuildCraftText(string itemName) { return $"Скрафчено: {itemName}"; }
}
