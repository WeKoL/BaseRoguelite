using Xunit;

public sealed class InventoryStateRemoveFromSlotTests
{
	private static InventoryItemDefinition Rock() => new InventoryItemDefinition("rock", 10, 1.0f);
	private static InventoryItemDefinition Metal() => new InventoryItemDefinition("metal", 10, 1.0f);

	[Fact]
	public void RemoveFromSlot_RemovesOnlyFromSpecifiedSlot()
	{
		InventoryState inventory = new InventoryState(5, 100.0f);

		inventory.TryAddItem(Rock(), 12);   // slot 0 = 10, slot 1 = 2
		inventory.TryAddItem(Metal(), 3);   // slot 2 = 3

		int removed = inventory.RemoveFromSlot(1, 2);

		Assert.Equal(2, removed);

		Assert.False(inventory.Slots[0].IsEmpty);
		Assert.Equal("rock", inventory.Slots[0].ItemId);
		Assert.Equal(10, inventory.Slots[0].Amount);

		Assert.True(inventory.Slots[1].IsEmpty);

		Assert.False(inventory.Slots[2].IsEmpty);
		Assert.Equal("metal", inventory.Slots[2].ItemId);
		Assert.Equal(3, inventory.Slots[2].Amount);
	}

	[Fact]
	public void RemoveFromSlot_RemovesPartialAmount_FromSpecifiedSlot()
	{
		InventoryState inventory = new InventoryState(5, 100.0f);

		inventory.TryAddItem(Rock(), 15); // slot 0 = 10, slot 1 = 5

		int removed = inventory.RemoveFromSlot(1, 3);

		Assert.Equal(3, removed);
		Assert.Equal(10, inventory.Slots[0].Amount);
		Assert.Equal(2, inventory.Slots[1].Amount);
		Assert.Equal("rock", inventory.Slots[1].ItemId);
	}

	[Fact]
	public void RemoveFromSlot_RemovesAllAvailable_WhenRequestedAmountIsGreaterThanSlotAmount()
	{
		InventoryState inventory = new InventoryState(5, 100.0f);

		inventory.TryAddItem(Rock(), 12); // slot 0 = 10, slot 1 = 2

		int removed = inventory.RemoveFromSlot(1, 10);

		Assert.Equal(2, removed);
		Assert.True(inventory.Slots[1].IsEmpty);
		Assert.Equal(10, inventory.Slots[0].Amount);
	}

	[Fact]
	public void RemoveFromSlot_ReturnsZero_ForInvalidSlotIndex()
	{
		InventoryState inventory = new InventoryState(3, 100.0f);
		inventory.TryAddItem(Rock(), 4);

		Assert.Equal(0, inventory.RemoveFromSlot(-1, 2));
		Assert.Equal(0, inventory.RemoveFromSlot(99, 2));
	}

	[Fact]
	public void RemoveFromSlot_ReturnsZero_ForEmptySlot()
	{
		InventoryState inventory = new InventoryState(3, 100.0f);

		int removed = inventory.RemoveFromSlot(1, 2);

		Assert.Equal(0, removed);
	}

	[Fact]
	public void RemoveFromSlot_ReturnsZero_WhenAmountIsNotPositive()
	{
		InventoryState inventory = new InventoryState(3, 100.0f);
		inventory.TryAddItem(Rock(), 4);

		Assert.Equal(0, inventory.RemoveFromSlot(0, 0));
		Assert.Equal(0, inventory.RemoveFromSlot(0, -3));
		Assert.Equal(4, inventory.Slots[0].Amount);
	}
}
