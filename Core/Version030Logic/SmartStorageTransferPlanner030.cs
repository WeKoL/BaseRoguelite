using System.Collections.Generic;
using System.Linq;

public sealed class SmartStorageTransferPlan030
{
	public IReadOnlyList<string> KeepInInventory { get; }
	public IReadOnlyList<string> MoveToStorage { get; }
	public SmartStorageTransferPlan030(IEnumerable<string> keep, IEnumerable<string> move)
	{
		KeepInInventory = (keep ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
		MoveToStorage = (move ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
	}
}

public static class SmartStorageTransferPlanner030
{
	public static SmartStorageTransferPlan030 BuildPlan(IEnumerable<InventorySlotState> slots, IEnumerable<string> protectedItemIds)
	{
		HashSet<string> protectedSet = new((protectedItemIds ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)));
		List<string> keep = new();
		List<string> move = new();
		if (slots != null)
		{
			foreach (InventorySlotState slot in slots)
			{
				if (slot == null || slot.IsEmpty) continue;
				if (protectedSet.Contains(slot.ItemId)) keep.Add(slot.ItemId);
				else move.Add(slot.ItemId);
			}
		}
		return new SmartStorageTransferPlan030(keep, move);
	}
}
