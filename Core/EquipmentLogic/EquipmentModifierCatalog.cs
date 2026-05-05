using System;
using System.Collections.Generic;

public static class EquipmentModifierCatalog
{
	private static readonly Dictionary<EquipmentModifierId, EquipmentModifierDefinition> Definitions = new()
	{
		{
			EquipmentModifierId.None,
			new EquipmentModifierDefinition(
				EquipmentModifierId.None,
				string.Empty,
				1.0f)
		},
		{
			EquipmentModifierId.Worn,
			new EquipmentModifierDefinition(
				EquipmentModifierId.Worn,
				"Изношенный",
				0.8f)
		},
		{
			EquipmentModifierId.Reinforced,
			new EquipmentModifierDefinition(
				EquipmentModifierId.Reinforced,
				"Укреплённый",
				1.2f)
		},
		{
			EquipmentModifierId.Calibrated,
			new EquipmentModifierDefinition(
				EquipmentModifierId.Calibrated,
				"Откалиброванный",
				1.1f,
				new[]
				{
					EquipmentSlotId.PrimaryWeapon,
					EquipmentSlotId.SecondaryWeapon
				})
		}
	};

	public static EquipmentModifierDefinition Get(EquipmentModifierId modifierId)
	{
		if (Definitions.TryGetValue(modifierId, out EquipmentModifierDefinition definition))
			return definition;

		return Definitions[EquipmentModifierId.None];
	}

	public static bool IsAllowedForSlot(EquipmentModifierId modifierId, EquipmentSlotId slotId)
	{
		return Get(modifierId).IsAllowedFor(slotId);
	}

	public static int ApplyDurabilityModifier(EquipmentModifierId modifierId, int baseMaxDurability)
	{
		if (baseMaxDurability <= 0)
			return 0;

		EquipmentModifierDefinition definition = Get(modifierId);
		int adjusted = (int)MathF.Round(baseMaxDurability * definition.DurabilityMultiplier);

		return Math.Max(1, adjusted);
	}

	public static string BuildDisplayName(string baseDisplayName, EquipmentModifierId modifierId)
	{
		if (string.IsNullOrWhiteSpace(baseDisplayName))
			return string.Empty;

		EquipmentModifierDefinition definition = Get(modifierId);
		if (string.IsNullOrWhiteSpace(definition.Prefix))
			return baseDisplayName;

		return $"{definition.Prefix} {baseDisplayName}";
	}
}
