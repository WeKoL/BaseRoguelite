public static class ItemComparisonTextBuilder
{
	public static string CompareDurability(int candidateDurability, int equippedDurability)
	{
		int delta = candidateDurability - equippedDurability; if (delta > 0) return $"Прочность +{delta}"; if (delta < 0) return $"Прочность {delta}"; return "Прочность без изменений";
	}
}
