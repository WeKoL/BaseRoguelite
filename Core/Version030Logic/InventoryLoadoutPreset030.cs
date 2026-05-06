using System.Collections.Generic;
using System.Linq;

public sealed class InventoryLoadoutPreset030
{
	private readonly Dictionary<int, string> _slotPurpose = new();
	public string Name { get; }
	public IReadOnlyDictionary<int, string> SlotPurpose => _slotPurpose;

	public InventoryLoadoutPreset030(string name)
	{
		Name = string.IsNullOrWhiteSpace(name) ? "Выход" : name;
	}

	public bool AssignPurpose(int slotIndex, string purpose)
	{
		if (slotIndex < 0 || string.IsNullOrWhiteSpace(purpose)) return false;
		_slotPurpose[slotIndex] = purpose.Trim();
		return true;
	}

	public IReadOnlyList<int> FindSlotsByPurpose(string purpose)
	{
		if (string.IsNullOrWhiteSpace(purpose)) return new List<int>();
		return _slotPurpose.Where(x => x.Value == purpose).Select(x => x.Key).OrderBy(x => x).ToList();
	}
}
