public static class DebugStartingLoadout
{
	public static void FillInventory(PlayerInventoryState inventory, ItemCatalog catalog)
	{
		if (inventory == null || catalog == null) return;
		TryAdd(inventory, catalog, "rock", 6);
		TryAdd(inventory, catalog, "wooden_plank", 8);
		TryAdd(inventory, catalog, "metal", 5);
		TryAdd(inventory, catalog, "simple_medkit", 2);
	}
	private static void TryAdd(PlayerInventoryState inventory, ItemCatalog catalog, string itemId, int amount)
	{
		if (catalog.TryGet(itemId, out ItemData item)) inventory.TryAddItem(item, amount);
	}
}
