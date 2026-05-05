public static class InventoryHintTextBuilder
{
	public static string BuildWeightHint(InventoryState inventory)
	{
		if (inventory == null) return string.Empty; return $"Вес: {inventory.GetCurrentWeight():0.##}/{inventory.MaxCarryWeight:0.##} кг";
	}
}
