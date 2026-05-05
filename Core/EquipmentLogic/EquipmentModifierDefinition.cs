using System.Collections.Generic;

public sealed class EquipmentModifierDefinition
{
	private readonly HashSet<EquipmentSlotId> _allowedSlots;

	public EquipmentModifierId Id { get; }
	public string Prefix { get; }
	public float DurabilityMultiplier { get; }

	public EquipmentModifierDefinition(
		EquipmentModifierId id,
		string prefix,
		float durabilityMultiplier,
		IEnumerable<EquipmentSlotId> allowedSlots = null)
	{
		Id = id;
		Prefix = prefix ?? string.Empty;
		DurabilityMultiplier = durabilityMultiplier <= 0f ? 1.0f : durabilityMultiplier;
		_allowedSlots = allowedSlots != null
			? new HashSet<EquipmentSlotId>(allowedSlots)
			: null;
	}

	public bool IsAllowedFor(EquipmentSlotId slotId)
	{
		if (_allowedSlots == null)
			return true;

		return _allowedSlots.Contains(slotId);
	}
}
