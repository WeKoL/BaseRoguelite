public static class GameSnapshotBuilder
{
	public static SaveGameData Build(PlayerController player, BaseRoot baseRoot)
	{
		SaveGameData data = new();

		if (player?.Stats != null)
		{
			data.PlayerHealth = player.Stats.CurrentHealth;
			data.PlayerMaxHealth = player.Stats.MaxHealth;
			data.PlayerStamina = player.Stats.CurrentStamina;
			data.PlayerMaxStamina = player.Stats.MaxStamina;
		}

		if (player?.Needs != null)
		{
			data.PlayerFood = player.Needs.Food;
			data.PlayerMaxFood = player.Needs.MaxFood;
			data.PlayerWater = player.Needs.Water;
			data.PlayerMaxWater = player.Needs.MaxWater;
		}

		if (player?.Inventory != null)
		{
			foreach (InventorySlotState slot in player.Inventory.Slots)
			{
				if (slot != null && !slot.IsEmpty)
				{
					data.InventoryItems.Add(new SaveSlotItemData
					{
						ItemId = slot.ItemId,
						Amount = slot.Amount,
						MaxStackSize = slot.MaxStackSize,
						WeightPerUnit = slot.WeightPerUnit
					});
				}
			}
		}

		if (baseRoot?.Storage != null)
		{
			foreach (StorageEntryState entry in baseRoot.Storage.GetLogicState().Entries)
			{
				if (entry != null && !string.IsNullOrWhiteSpace(entry.ItemId) && entry.Amount > 0)
				{
					data.StorageItems.Add(new SaveSlotItemData
					{
						ItemId = entry.ItemId,
						Amount = entry.Amount,
						MaxStackSize = entry.MaxStackSize,
						WeightPerUnit = 1f
					});
				}
			}
		}

		data.BaseLevel = baseRoot?.Progress?.BaseLevel ?? 1;
		return data;
	}
}
