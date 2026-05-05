using Xunit;

public sealed class Version30InventoryTests
{
	[Fact]
	public void MoveOrMerge_MergesSameItemStacks()
	{
		InventoryState inventory = new(3, 50);
		InventoryItemDefinition wood = new("wood", 10, 1);
		inventory.TryAddItem(wood, 6);
		inventory.TrySplitStack(0, 2);
		InventoryMoveResult result = InventoryMoveLogic.TryMoveOrMerge(inventory, 1, 0);
		Assert.True(result.Succeeded);
		Assert.Equal(6, inventory.Slots[0].Amount);
		Assert.True(inventory.Slots[1].IsEmpty);
	}

	[Fact]
	public void Filter_ReturnsOnlyNonEmptySlotsWithIndexes()
	{
		InventoryState inventory = new(4, 50);
		inventory.TryAddItem(new InventoryItemDefinition("metal", 5, 1), 2);
		var slots = InventoryFilterLogic.GetNonEmptySlots(inventory);
		Assert.Single(slots);
		Assert.Equal(0, slots[0].SlotIndex);
	}

	[Fact]
	public void LoadoutScore_CountsBrokenEquipment()
	{
		EquipmentState equipment = new();
		equipment.EquipToSlot(EquipmentSlotId.Head, new EquipmentItemDefinition("helmet", EquipmentSlotId.Head, 10));
		equipment.ReduceDurability(EquipmentSlotId.Head, 10);
		EquipmentLoadoutScore score = EquipmentLoadoutScoreCalculator.Calculate(equipment);
		Assert.Equal(1, score.EquippedCount);
		Assert.Equal(1, score.BrokenCount);
	}
}
