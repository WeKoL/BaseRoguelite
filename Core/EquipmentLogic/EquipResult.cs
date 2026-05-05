public sealed class EquipResult
{
	public bool Succeeded { get; }
	public EquipmentEquipFailureReason FailureReason { get; }
	public EquippedItemState EquippedItem { get; }
	public EquippedItemState ReplacedItem { get; }

	private EquipResult(
		bool succeeded,
		EquipmentEquipFailureReason failureReason,
		EquippedItemState equippedItem,
		EquippedItemState replacedItem)
	{
		Succeeded = succeeded;
		FailureReason = failureReason;
		EquippedItem = equippedItem;
		ReplacedItem = replacedItem;
	}

	public static EquipResult Success(EquippedItemState equippedItem, EquippedItemState replacedItem)
	{
		return new EquipResult(true, EquipmentEquipFailureReason.None, equippedItem, replacedItem);
	}

	public static EquipResult Fail(EquipmentEquipFailureReason failureReason)
	{
		return new EquipResult(false, failureReason, null, null);
	}
}
