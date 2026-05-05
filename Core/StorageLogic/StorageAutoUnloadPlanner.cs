using System.Collections.Generic;
public static class StorageAutoUnloadPlanner
{
	public static IReadOnlyList<InventorySlotSnapshot> BuildPlan(InventoryState inventory, IReadOnlyDictionary<string, ItemCategory> categories, IEnumerable<StorageAutoUnloadRule> rules)
	{
		List<InventorySlotSnapshot> result = new(); if (inventory == null || categories == null || rules == null) return result;
		Dictionary<ItemCategory,bool> ruleMap = new(); foreach (StorageAutoUnloadRule r in rules) if (r != null) ruleMap[r.Category] = r.ShouldUnload;
		foreach (InventorySlotSnapshot slot in InventoryFilterLogic.GetNonEmptySlots(inventory)) if (categories.TryGetValue(slot.ItemId, out ItemCategory category) && ruleMap.TryGetValue(category, out bool unload) && unload) result.Add(slot);
		return result;
	}
}
