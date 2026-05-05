using Xunit;

namespace BaseRoguelite.Tests.Equipment;

public class EquipmentStateTests
{
	private static EquipmentItemDefinition CreatePrimaryWeapon(
		string itemId = "pipe_rifle",
		int maxDurability = 100)
	{
		return new EquipmentItemDefinition(itemId, EquipmentSlotId.PrimaryWeapon, maxDurability);
	}

	private static EquipmentItemDefinition CreateHead(
		string itemId = "scrap_helmet",
		int maxDurability = 80)
	{
		return new EquipmentItemDefinition(itemId, EquipmentSlotId.Head, maxDurability);
	}

	[Fact]
	public void EquipToSlot_Fails_WhenItemDoesNotMatchRequestedSlot()
	{
		var equipment = new EquipmentState();
		var helmet = CreateHead();

		EquipResult result = equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, helmet);

		Assert.False(result.Succeeded);
		Assert.Equal(EquipmentEquipFailureReason.SlotMismatch, result.FailureReason);
		Assert.Null(equipment.GetEquippedItem(EquipmentSlotId.PrimaryWeapon));
	}

	[Fact]
	public void EquipToSlot_EquipsItem_WhenSlotMatches()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon();

		EquipResult result = equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, weapon);

		Assert.True(result.Succeeded);
		Assert.Equal(EquipmentEquipFailureReason.None, result.FailureReason);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal("pipe_rifle", result.EquippedItem.ItemId);
		Assert.Equal(100, result.EquippedItem.CurrentDurability);
		Assert.Equal(100, result.EquippedItem.MaxDurability);

		EquippedItemState equipped = equipment.GetEquippedItem(EquipmentSlotId.PrimaryWeapon);
		Assert.NotNull(equipped);
		Assert.Equal("pipe_rifle", equipped.ItemId);
	}

	[Fact]
	public void EquipToSlot_ReturnsReplacedItem_WhenReplacingExistingItem()
	{
		var equipment = new EquipmentState();
		var oldWeapon = CreatePrimaryWeapon("rusty_pistol", 60);
		var newWeapon = CreatePrimaryWeapon("pipe_rifle", 100);

		EquipResult firstResult = equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, oldWeapon);
		EquipResult secondResult = equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, newWeapon);

		Assert.True(firstResult.Succeeded);
		Assert.True(secondResult.Succeeded);

		Assert.NotNull(secondResult.ReplacedItem);
		Assert.Equal("rusty_pistol", secondResult.ReplacedItem.ItemId);
		Assert.Equal(60, secondResult.ReplacedItem.MaxDurability);

		EquippedItemState equipped = equipment.GetEquippedItem(EquipmentSlotId.PrimaryWeapon);
		Assert.NotNull(equipped);
		Assert.Equal("pipe_rifle", equipped.ItemId);
		Assert.Equal(100, equipped.CurrentDurability);
	}

	[Fact]
	public void ReduceDurability_DoesNotGoBelowZero()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(maxDurability: 100);

		equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, weapon);

		int durability = equipment.ReduceDurability(EquipmentSlotId.PrimaryWeapon, 150);

		Assert.Equal(0, durability);
		Assert.Equal(0, equipment.GetCurrentDurability(EquipmentSlotId.PrimaryWeapon));
	}

	[Fact]
	public void ReduceDurability_LeavesBrokenItemEquippedAtZeroDurability()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(maxDurability: 100);

		equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, weapon);
		equipment.ReduceDurability(EquipmentSlotId.PrimaryWeapon, 100);

		EquippedItemState equipped = equipment.GetEquippedItem(EquipmentSlotId.PrimaryWeapon);

		Assert.NotNull(equipped);
		Assert.Equal("pipe_rifle", equipped.ItemId);
		Assert.True(equipped.IsBroken);
		Assert.Equal(0, equipped.CurrentDurability);
	}

	[Fact]
	public void Unequip_RemovesItemFromSlot_AndReturnsIt()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon();

		equipment.EquipToSlot(EquipmentSlotId.PrimaryWeapon, weapon);

		EquippedItemState removed = equipment.Unequip(EquipmentSlotId.PrimaryWeapon);

		Assert.NotNull(removed);
		Assert.Equal("pipe_rifle", removed.ItemId);
		Assert.Null(equipment.GetEquippedItem(EquipmentSlotId.PrimaryWeapon));
	}
}
