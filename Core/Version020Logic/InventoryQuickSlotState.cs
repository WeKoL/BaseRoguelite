using System;
using System.Collections.Generic;

public sealed class InventoryQuickSlotState
{
	private readonly Dictionary<int, int> _slotToInventoryIndex = new();
	public int QuickSlotCount { get; }
	public int SelectedQuickSlot { get; private set; }

	public InventoryQuickSlotState(int quickSlotCount = 4)
	{
		QuickSlotCount = Math.Max(1, quickSlotCount);
		SelectedQuickSlot = 0;
	}

	public bool Assign(int quickSlotIndex, int inventorySlotIndex)
	{
		if (quickSlotIndex < 0 || quickSlotIndex >= QuickSlotCount) return false;
		if (inventorySlotIndex < 0) return false;
		_slotToInventoryIndex[quickSlotIndex] = inventorySlotIndex;
		return true;
	}

	public bool TryGetInventorySlot(int quickSlotIndex, out int inventorySlotIndex)
	{
		inventorySlotIndex = -1;
		if (quickSlotIndex < 0 || quickSlotIndex >= QuickSlotCount) return false;
		return _slotToInventoryIndex.TryGetValue(quickSlotIndex, out inventorySlotIndex);
	}

	public int Select(int quickSlotIndex)
	{
		SelectedQuickSlot = Math.Clamp(quickSlotIndex, 0, QuickSlotCount - 1);
		return SelectedQuickSlot;
	}

	public int Cycle(int direction)
	{
		int step = direction >= 0 ? 1 : -1;
		SelectedQuickSlot = (SelectedQuickSlot + step + QuickSlotCount) % QuickSlotCount;
		return SelectedQuickSlot;
	}
}
