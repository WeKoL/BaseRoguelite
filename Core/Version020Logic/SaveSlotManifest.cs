using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SaveSlotManifest
{
	private readonly List<SaveSlotDescriptor> _slots = new();
	public IReadOnlyList<SaveSlotDescriptor> Slots => _slots;
	public int Count => _slots.Count;

	public bool Add(SaveSlotDescriptor descriptor)
	{
		if (descriptor == null || string.IsNullOrWhiteSpace(descriptor.FileName)) return false;
		if (_slots.Any(s => s.SlotIndex == descriptor.SlotIndex || s.FileName == descriptor.FileName)) return false;
		_slots.Add(descriptor);
		return true;
	}

	public SaveSlotDescriptor GetHighestSlot()
	{
		if (_slots.Count == 0) return null;
		return _slots.OrderByDescending(s => s.SlotIndex).FirstOrDefault();
	}
}
