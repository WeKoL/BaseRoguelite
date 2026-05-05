public sealed class SaveSlotDescriptor
{
	public int SlotIndex { get; }
	public string FileName { get; }
	public SaveSlotDescriptor(int slotIndex, string fileName) { SlotIndex = slotIndex <= 0 ? 1 : slotIndex; FileName = string.IsNullOrWhiteSpace(fileName) ? "save_1.json" : fileName; }
}
