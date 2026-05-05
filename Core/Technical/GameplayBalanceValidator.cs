using System.Collections.Generic;
public static class GameplayBalanceValidator
{
	public static IReadOnlyList<GameplayBalanceWarning> ValidateInventoryAndStorage(InventoryState inventory, StorageState storage)
	{
		List<GameplayBalanceWarning> warnings = new();
		if (inventory == null) warnings.Add(new GameplayBalanceWarning("inventory_missing", "Инвентарь не создан.")); else { if (inventory.MaxSlots < 4) warnings.Add(new GameplayBalanceWarning("inventory_too_small", "В инвентаре слишком мало слотов.")); if (inventory.MaxCarryWeight < 5f) warnings.Add(new GameplayBalanceWarning("carry_weight_too_low", "Переносимый вес слишком мал.")); }
		if (storage == null) warnings.Add(new GameplayBalanceWarning("storage_missing", "Хранилище не создано.")); else if (storage.HasEntryLimit && storage.MaxEntries < 5) warnings.Add(new GameplayBalanceWarning("storage_too_small", "Хранилище слишком маленькое."));
		return warnings;
	}
}
