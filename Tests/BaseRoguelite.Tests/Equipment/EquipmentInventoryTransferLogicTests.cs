using Xunit;

namespace BaseRoguelite.Tests.Equipment;

public class EquipmentInventoryTransferLogicTests
{
	private static EquipmentItemDefinition CreateHelmetEquipmentDefinition()
	{
		return new EquipmentItemDefinition(
			"scrap_helmet",
			EquipmentSlotId.Head,
			100,
			"Шлем из лома");
	}

	private static InventoryItemDefinition CreateHelmetInventoryDefinition()
	{
		return new InventoryItemDefinition(
			"scrap_helmet",
			1,
			3.0f);
	}

	[Fact]
	public void TryUnequipToInventory_Fails_WhenInventoryHasNoFreeCapacity()
	{
		EquipmentState equipment = new EquipmentState();
		InventoryState inventory = new InventoryState(1, 100.0f);

		inventory.TryAddItem(new InventoryItemDefinition("rock", 99, 1.0f), 1);

		equipment.EquipToSlot(
			EquipmentSlotId.Head,
			CreateHelmetEquipmentDefinition());

		UnequipToInventoryResult result = EquipmentInventoryTransferLogic.TryUnequipToInventory(
			equipment,
			inventory,
			EquipmentSlotId.Head,
			CreateHelmetInventoryDefinition());

		Assert.False(result.Succeeded);
		Assert.Equal(UnequipToInventoryFailureReason.InventoryFull, result.FailureReason);
		Assert.NotNull(equipment.GetEquippedItem(EquipmentSlotId.Head));
		Assert.Equal("scrap_helmet", equipment.GetEquippedItem(EquipmentSlotId.Head).ItemId);
		Assert.Equal(0, inventory.GetTotalAmount("scrap_helmet"));
		Assert.Equal(1, inventory.GetTotalAmount("rock"));
	}

	[Fact]
	public void TryUnequipToInventory_MovesItem_WhenInventoryHasSpace()
	{
		EquipmentState equipment = new EquipmentState();
		InventoryState inventory = new InventoryState(5, 100.0f);

		equipment.EquipToSlot(
			EquipmentSlotId.Head,
			CreateHelmetEquipmentDefinition());

		UnequipToInventoryResult result = EquipmentInventoryTransferLogic.TryUnequipToInventory(
			equipment,
			inventory,
			EquipmentSlotId.Head,
			CreateHelmetInventoryDefinition());

		Assert.True(result.Succeeded);
		Assert.Equal(UnequipToInventoryFailureReason.None, result.FailureReason);
		Assert.NotNull(result.UnequippedItem);
		Assert.Equal("scrap_helmet", result.UnequippedItem.ItemId);

		Assert.Null(equipment.GetEquippedItem(EquipmentSlotId.Head));
		Assert.Equal(1, inventory.GetTotalAmount("scrap_helmet"));
	}
}
