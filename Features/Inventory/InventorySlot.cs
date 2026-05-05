using Godot;

public partial class InventorySlot : RefCounted
{
	public ItemEntry Entry { get; private set; }

	public bool IsEmpty => Entry == null || Entry.IsEmpty() || Entry.Item == null;

	public void Clear()
	{
		Entry = null;
	}

	public void SetEntry(ItemEntry entry)
	{
		Entry = entry;
	}
}
