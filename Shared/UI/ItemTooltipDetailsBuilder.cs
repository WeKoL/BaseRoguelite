public static class ItemTooltipDetailsBuilder
{
	public static string BuildWeightLine(int amount, float weightPerUnit) { float total = amount <= 0 || weightPerUnit <= 0f ? 0f : amount * weightPerUnit; return $"Вес: {total:0.##} кг"; }
	public static string BuildDurabilityLine(int current, int max) { if (max <= 0) return "Прочность: —"; return $"Прочность: {System.Math.Clamp(current, 0, max)}/{max}"; }
}
