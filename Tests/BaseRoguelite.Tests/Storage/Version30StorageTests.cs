using System.Collections.Generic;
using Xunit;

public sealed class Version30StorageTests
{
	[Fact]
	public void Storage_WithEntryLimit_RejectsOverflow()
	{
		StorageState storage = new(1);
		int added = storage.TryAddItem("wood", 15, 10);
		Assert.Equal(10, added);
		Assert.False(storage.CanFit("stone", 1, 10));
	}

	[Fact]
	public void Search_FindsEntriesByPartialId()
	{
		StorageState storage = new();
		storage.AddItem("metal_plate", 1, 10);
		storage.AddItem("wood", 1, 10);
		Assert.Single(StorageSearchLogic.SearchByItemId(storage, "metal"));
	}

	[Fact]
	public void AutoUnloadPlanner_UsesCategoryRules()
	{
		InventoryState inventory = new(3, 50);
		inventory.TryAddItem(new InventoryItemDefinition("wood", 10, 1), 2);
		Dictionary<string, ItemCategory> categories = new() { ["wood"] = ItemCategory.Material };
		var plan = StorageAutoUnloadPlanner.BuildPlan(inventory, categories, new[] { new StorageAutoUnloadRule(ItemCategory.Material, true) });
		Assert.Single(plan);
	}
}
