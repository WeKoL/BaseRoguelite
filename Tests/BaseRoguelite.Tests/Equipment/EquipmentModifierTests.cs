using Xunit;

namespace BaseRoguelite.Tests.Equipment;

public class EquipmentModifierTests
{
	private static EquipmentItemDefinition CreatePrimaryWeapon(
		string itemId = "pipe_rifle",
		int maxDurability = 100,
		string displayName = "Самодельная винтовка")
	{
		return new EquipmentItemDefinition(
			itemId,
			EquipmentSlotId.PrimaryWeapon,
			maxDurability,
			displayName);
	}

	private static EquipmentItemDefinition CreateHead(
		string itemId = "scrap_helmet",
		int maxDurability = 80,
		string displayName = "Шлем из лома")
	{
		return new EquipmentItemDefinition(
			itemId,
			EquipmentSlotId.Head,
			maxDurability,
			displayName);
	}

	[Fact]
	public void EquipToSlot_AppliesNegativeModifier_ToDurability()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(maxDurability: 100);

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			weapon,
			EquipmentModifierId.Worn);

		Assert.True(result.Succeeded);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal(80, result.EquippedItem.MaxDurability);
		Assert.Equal(80, result.EquippedItem.CurrentDurability);
	}

	[Fact]
	public void EquipToSlot_AppliesPositiveModifier_ToDurability()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(maxDurability: 100);

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			weapon,
			EquipmentModifierId.Reinforced);

		Assert.True(result.Succeeded);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal(120, result.EquippedItem.MaxDurability);
		Assert.Equal(120, result.EquippedItem.CurrentDurability);
	}

	[Fact]
	public void EquipToSlot_BuildsPrefixedDisplayName_FromModifier()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(displayName: "Самодельная винтовка");

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			weapon,
			EquipmentModifierId.Calibrated);

		Assert.True(result.Succeeded);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal("Откалиброванный Самодельная винтовка", result.EquippedItem.DisplayName);
	}

	[Fact]
	public void EquipToSlot_Fails_WhenModifierIsNotAllowedForSlot()
	{
		var equipment = new EquipmentState();
		var helmet = CreateHead();

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.Head,
			helmet,
			EquipmentModifierId.Calibrated);

		Assert.False(result.Succeeded);
		Assert.Equal(EquipmentEquipFailureReason.ModifierNotAllowed, result.FailureReason);
		Assert.Null(equipment.GetEquippedItem(EquipmentSlotId.Head));
	}

	[Fact]
	public void EquipToSlot_WithoutModifier_UsesBaseDisplayName()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(displayName: "Самодельная винтовка");

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			weapon);

		Assert.True(result.Succeeded);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal("Самодельная винтовка", result.EquippedItem.DisplayName);
		Assert.Equal(EquipmentModifierId.None, result.EquippedItem.ModifierId);
	}

	[Fact]
	public void NegativeModifier_DoesNotMakeDurabilityInvalid()
	{
		var equipment = new EquipmentState();
		var weapon = CreatePrimaryWeapon(maxDurability: 1);

		EquipResult result = equipment.EquipToSlot(
			EquipmentSlotId.PrimaryWeapon,
			weapon,
			EquipmentModifierId.Worn);

		Assert.True(result.Succeeded);
		Assert.NotNull(result.EquippedItem);
		Assert.Equal(1, result.EquippedItem.MaxDurability);
		Assert.Equal(1, result.EquippedItem.CurrentDurability);
	}
}
