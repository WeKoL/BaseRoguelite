using System.Linq;
public static class InventorySortLogic
{
	public static InventorySlotSnapshot[] SnapshotSortedByItemId(InventoryState inventory)
	{
		return InventoryFilterLogic.GetNonEmptySlots(inventory).OrderBy(s => s.ItemId).ThenBy(s => s.SlotIndex).ToArray();
	}
}
