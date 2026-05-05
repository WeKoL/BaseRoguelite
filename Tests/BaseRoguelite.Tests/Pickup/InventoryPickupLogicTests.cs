using Xunit;

namespace BaseRoguelite.Tests.Pickup;

public class InventoryPickupLogicTests
{
	private static InventoryItemDefinition CreateItem(
		string id = "rock",
		int maxStackSize = 99,
		float weight = 1.0f)
	{
		return new InventoryItemDefinition(id, maxStackSize, weight);
	}

	[Fact]
	public void TryPickup_AddsAll_WhenInventoryHasEnoughCapacity()
	{
		var inventory = new InventoryState(4, 60.0f);
		var rock = CreateItem();

		PickupResult result = InventoryPickupLogic.TryPickup(inventory, rock, 10);

		Assert.True(result.Succeeded);
		Assert.Equal(PickupFailureReason.None, result.FailureReason);
		Assert.Equal(10, result.PickedAmount);
		Assert.Equal(10, inventory.GetTotalAmount("rock"));
		Assert.Equal(10.0f, inventory.GetCurrentWeight());
	}

	[Fact]
	public void TryPickup_Fails_WhenWeightLimitWouldBeExceeded()
	{
		var inventory = new InventoryState(4, 5.0f);
		var rock = CreateItem(weight: 1.0f);

		PickupResult result = InventoryPickupLogic.TryPickup(inventory, rock, 6);

		Assert.False(result.Succeeded);
		Assert.Equal(PickupFailureReason.NotEnoughWeightCapacity, result.FailureReason);
		Assert.Equal(0, result.PickedAmount);
		Assert.Equal(0, inventory.GetTotalAmount("rock"));
		Assert.Equal(0.0f, inventory.GetCurrentWeight());
	}

	[Fact]
	public void TryPickup_Fails_WhenNoSlotCapacityForRequestedAmount()
	{
		var inventory = new InventoryState(1, 10.0f);
		var artifact = CreateItem(id: "artifact", maxStackSize: 1, weight: 1.0f);

		PickupResult result = InventoryPickupLogic.TryPickup(inventory, artifact, 2);

		Assert.False(result.Succeeded);
		Assert.Equal(PickupFailureReason.NotEnoughSlots, result.FailureReason);
		Assert.Equal(0, result.PickedAmount);
		Assert.True(inventory.Slots[0].IsEmpty);
	}

	[Fact]
	public void TryPickup_UsesExistingStackBeforeConsumingEmptySlot()
	{
		var inventory = new InventoryState(2, 60.0f);
		var rock = CreateItem(maxStackSize: 10, weight: 1.0f);
		inventory.TryAddItem(rock, 8);

		PickupResult result = InventoryPickupLogic.TryPickup(inventory, rock, 5);

		Assert.True(result.Succeeded);
		Assert.Equal(5, result.PickedAmount);
		Assert.Equal(10, inventory.Slots[0].Amount);
		Assert.Equal(3, inventory.Slots[1].Amount);
		Assert.Equal("rock", inventory.Slots[1].ItemId);
	}
}
