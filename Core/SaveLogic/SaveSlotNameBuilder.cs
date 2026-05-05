public static class SaveSlotNameBuilder
{
	public static SaveSlotDescriptor Build(int slotIndex) { int safeIndex = slotIndex <= 0 ? 1 : slotIndex; return new SaveSlotDescriptor(safeIndex, $"save_{safeIndex}.json"); }
}
