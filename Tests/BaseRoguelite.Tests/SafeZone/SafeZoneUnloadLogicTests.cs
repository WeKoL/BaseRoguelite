using System.Linq;
using Xunit;

namespace BaseRoguelite.Tests.SafeZone;

public class SafeZoneUnloadLogicTests
{
	[Fact]
	public void UnloadAll_MovesEverythingFromInventory_AndReturnsPerItemSummary()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();

		inventory.TryAddItem(new InventoryItemDefinition("rock", 99, 1.0f), 6);
		inventory.TryAddItem(new InventoryItemDefinition("metal", 99, 1.0f), 4);
		inventory.TryAddItem(new InventoryItemDefinition("wooden_plank", 99, 1.0f), 8);

		SafeZoneUnloadResult result = SafeZoneUnloadLogic.UnloadAll(inventory, storage);

		Assert.True(result.MovedAnything);
		Assert.True(result.FullyUnloaded);
		Assert.Equal(18, result.ExpectedMovedAmount);
		Assert.Equal(18, result.TotalMovedAmount);

		Assert.Equal(0, inventory.GetTotalAmount("rock"));
		Assert.Equal(0, inventory.GetTotalAmount("metal"));
		Assert.Equal(0, inventory.GetTotalAmount("wooden_plank"));

		Assert.Equal(6, storage.GetTotalAmount("rock"));
		Assert.Equal(4, storage.GetTotalAmount("metal"));
		Assert.Equal(8, storage.GetTotalAmount("wooden_plank"));

		SafeZoneUnloadEntry rock = result.Entries.Single(x => x.ItemId == "rock");
		SafeZoneUnloadEntry metal = result.Entries.Single(x => x.ItemId == "metal");
		SafeZoneUnloadEntry plank = result.Entries.Single(x => x.ItemId == "wooden_plank");

		Assert.Equal(6, rock.ExpectedAmount);
		Assert.Equal(6, rock.MovedAmount);
		Assert.Equal(6, rock.FinalStorageAmount);

		Assert.Equal(4, metal.ExpectedAmount);
		Assert.Equal(4, metal.MovedAmount);
		Assert.Equal(4, metal.FinalStorageAmount);

		Assert.Equal(8, plank.ExpectedAmount);
		Assert.Equal(8, plank.MovedAmount);
		Assert.Equal(8, plank.FinalStorageAmount);
	}

	[Fact]
	public void UnloadAll_AddsToExistingStorage_WithoutReducingPreviousAmounts()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();

		storage.AddItem("rock", 10, 99);
		storage.AddItem("metal", 3, 99);

		inventory.TryAddItem(new InventoryItemDefinition("rock", 99, 1.0f), 5);
		inventory.TryAddItem(new InventoryItemDefinition("metal", 99, 1.0f), 2);

		SafeZoneUnloadResult result = SafeZoneUnloadLogic.UnloadAll(inventory, storage);

		Assert.True(result.FullyUnloaded);
		Assert.Equal(15, storage.GetTotalAmount("rock"));
		Assert.Equal(5, storage.GetTotalAmount("metal"));

		SafeZoneUnloadEntry rock = result.Entries.Single(x => x.ItemId == "rock");
		SafeZoneUnloadEntry metal = result.Entries.Single(x => x.ItemId == "metal");

		Assert.Equal(5, rock.ExpectedAmount);
		Assert.Equal(5, rock.MovedAmount);
		Assert.Equal(15, rock.FinalStorageAmount);

		Assert.Equal(2, metal.ExpectedAmount);
		Assert.Equal(2, metal.MovedAmount);
		Assert.Equal(5, metal.FinalStorageAmount);
	}

	[Fact]
	public void UnloadAll_DoesNothing_WhenInventoryIsEmpty()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();

		SafeZoneUnloadResult result = SafeZoneUnloadLogic.UnloadAll(inventory, storage);

		Assert.False(result.MovedAnything);
		Assert.True(result.FullyUnloaded);
		Assert.Empty(result.Entries);
		Assert.Equal(0, result.ExpectedMovedAmount);
		Assert.Equal(0, result.TotalMovedAmount);
		Assert.Empty(storage.Entries);
	}
}
