using Xunit;

namespace BaseRoguelite.Tests.Inventory;

public class InventoryStateTests
{
	private static InventoryItemDefinition CreateMaterial(
		string id = "rock",
		int maxStackSize = 99,
		float weight = 1.0f)
	{
		return new InventoryItemDefinition(id, maxStackSize, weight);
	}

	[Fact]
	public void Constructor_Sets_DefaultLimits()
	{
		var inventory = new InventoryState(16, 60.0f);

		Assert.Equal(16, inventory.MaxSlots);
		Assert.Equal(60.0f, inventory.MaxCarryWeight);
		Assert.Equal(16, inventory.Slots.Count);
	}

	[Fact]
	public void EmptyInventory_HasZeroWeight()
	{
		var inventory = new InventoryState(16, 60.0f);

		Assert.Equal(0.0f, inventory.GetCurrentWeight());
		Assert.Equal(0.0f, inventory.GetWeightFillRatio());
	}

	[Fact]
	public void TryAddItem_AddsToEmptyInventory()
	{
		var inventory = new InventoryState(16, 60.0f);
		var rock = CreateMaterial();

		int added = inventory.TryAddItem(rock, 5);

		Assert.Equal(5, added);
		Assert.Equal(5.0f, inventory.GetCurrentWeight());
		Assert.False(inventory.Slots[0].IsEmpty);
		Assert.Equal(5, inventory.Slots[0].Amount);
		Assert.Equal("rock", inventory.Slots[0].ItemId);
	}

	[Fact]
	public void TryAddItem_StacksIntoExistingSlot()
	{
		var inventory = new InventoryState(16, 60.0f);
		var rock = CreateMaterial(maxStackSize: 99);

		int firstAdded = inventory.TryAddItem(rock, 5);
		int secondAdded = inventory.TryAddItem(rock, 7);

		Assert.Equal(5, firstAdded);
		Assert.Equal(7, secondAdded);
		Assert.Equal(12, inventory.Slots[0].Amount);
		Assert.Equal(12.0f, inventory.GetCurrentWeight());
	}

	[Fact]
	public void TryAddItem_RespectsWeightLimit()
	{
		var inventory = new InventoryState(16, 3.0f);
		var rock = CreateMaterial(weight: 1.0f);

		int added = inventory.TryAddItem(rock, 10);

		Assert.Equal(3, added);
		Assert.Equal(3.0f, inventory.GetCurrentWeight());
		Assert.Equal(3, inventory.Slots[0].Amount);
	}

	[Fact]
	public void TryAddItem_RespectsSlotLimit_WhenItemIsNotStackable()
	{
		var inventory = new InventoryState(2, 60.0f);
		var product = CreateMaterial(id: "artifact", maxStackSize: 1, weight: 1.0f);

		int added = inventory.TryAddItem(product, 3);

		Assert.Equal(2, added);
		Assert.False(inventory.Slots[0].IsEmpty);
		Assert.False(inventory.Slots[1].IsEmpty);
	}

	[Fact]
	public void TryAddItem_CanPartiallyFillSecondStack()
	{
		var inventory = new InventoryState(2, 60.0f);
		var rock = CreateMaterial(maxStackSize: 10, weight: 1.0f);

		int firstAdded = inventory.TryAddItem(rock, 8);
		int secondAdded = inventory.TryAddItem(rock, 5);

		Assert.Equal(8, firstAdded);
		Assert.Equal(5, secondAdded);

		Assert.Equal(10, inventory.Slots[0].Amount);
		Assert.Equal(3, inventory.Slots[1].Amount);
		Assert.Equal(13.0f, inventory.GetCurrentWeight());
	}

	[Fact]
	public void GetWeightFillRatio_ReturnsCorrectRatio()
	{
		var inventory = new InventoryState(16, 10.0f);
		var rock = CreateMaterial(weight: 1.0f);

		inventory.TryAddItem(rock, 4);

		Assert.Equal(0.4f, inventory.GetWeightFillRatio(), 3);
	}
}
