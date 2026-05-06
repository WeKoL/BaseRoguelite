public static class SaveGameValidator
{
	public static SaveValidationResult Validate(SaveGameData data)
	{
		SaveValidationResult result = new();
		if (data == null) { result.AddError("save_missing"); return result; }
		if (data.Version <= 0) result.AddError("invalid_version");
		if (data.PlayerMaxHealth <= 0) result.AddError("invalid_player_max_health");
		if (data.PlayerHealth < 0 || data.PlayerHealth > data.PlayerMaxHealth) result.AddError("invalid_player_health");
		if (data.PlayerMaxFood <= 0) result.AddError("invalid_player_max_food");
		if (data.PlayerFood < 0 || data.PlayerFood > data.PlayerMaxFood) result.AddError("invalid_player_food");
		if (data.PlayerMaxWater <= 0) result.AddError("invalid_player_max_water");
		if (data.PlayerWater < 0 || data.PlayerWater > data.PlayerMaxWater) result.AddError("invalid_player_water");
		foreach (SaveSlotItemData item in data.InventoryItems) ValidateSlotItem(item, result, "inventory");
		foreach (SaveSlotItemData item in data.StorageItems) ValidateSlotItem(item, result, "storage");
		return result;
	}

	private static void ValidateSlotItem(SaveSlotItemData item, SaveValidationResult result, string owner)
	{
		if (item == null) { result.AddError(owner + "_item_null"); return; }
		if (string.IsNullOrWhiteSpace(item.ItemId)) result.AddError(owner + "_item_id_missing");
		if (item.Amount <= 0) result.AddError(owner + "_item_amount_invalid");
		if (item.MaxStackSize <= 0) result.AddError(owner + "_item_stack_invalid");
	}
}
