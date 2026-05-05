using Xunit;

namespace BaseRoguelite.Tests.Transfer;

public class InventoryStorageTransferLogicTests
{
	private static InventoryItemDefinition CreateRock()
	{
		return new InventoryItemDefinition("rock", 99, 1.0f);
	}

	[Fact]
	public void MoveToStorage_MovesRequestedAmount_WhenAvailable()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();
		var rock = CreateRock();

		inventory.TryAddItem(rock, 10);

		var result = InventoryStorageTransferLogic.MoveToStorage(inventory, storage, rock, 4);

		Assert.True(result.MovedAnything);
		Assert.True(result.FullyMoved);
		Assert.Equal(4, result.MovedAmount);
		Assert.Equal(6, inventory.GetTotalAmount("rock"));
		Assert.Equal(4, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void MoveToStorage_MovesOnlyAvailableAmount()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();
		var rock = CreateRock();

		inventory.TryAddItem(rock, 3);

		var result = InventoryStorageTransferLogic.MoveToStorage(inventory, storage, rock, 8);

		Assert.Equal(3, result.MovedAmount);
		Assert.Equal(0, inventory.GetTotalAmount("rock"));
		Assert.Equal(3, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void MoveAllToStorage_EmptiesInventory_AndMovesEverything()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();

		var rock = new InventoryItemDefinition("rock", 99, 1.0f);
		var metal = new InventoryItemDefinition("metal", 99, 1.0f);
		var plank = new InventoryItemDefinition("wooden_plank", 99, 1.0f);

		inventory.TryAddItem(rock, 6);
		inventory.TryAddItem(metal, 4);
		inventory.TryAddItem(plank, 8);

		var result = InventoryStorageTransferLogic.MoveAllToStorage(inventory, storage);

		Assert.True(result.MovedAnything);
		Assert.Equal(18, result.MovedAmount);

		Assert.Equal(0, inventory.GetTotalAmount("rock"));
		Assert.Equal(0, inventory.GetTotalAmount("metal"));
		Assert.Equal(0, inventory.GetTotalAmount("wooden_plank"));

		Assert.Equal(6, storage.GetTotalAmount("rock"));
		Assert.Equal(4, storage.GetTotalAmount("metal"));
		Assert.Equal(8, storage.GetTotalAmount("wooden_plank"));
	}

	[Fact]
	public void MoveAllToStorage_DoesNothing_WhenInventoryIsEmpty()
	{
		var inventory = new InventoryState(16, 60.0f);
		var storage = new StorageState();

		var result = InventoryStorageTransferLogic.MoveAllToStorage(inventory, storage);

		Assert.False(result.MovedAnything);
		Assert.Equal(0, result.MovedAmount);
		Assert.Empty(storage.Entries);
	}
}
