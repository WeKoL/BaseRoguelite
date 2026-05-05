public sealed class UnequipToInventoryResult
{
	public bool Succeeded { get; }
	public UnequipToInventoryFailureReason FailureReason { get; }
	public EquippedItemState UnequippedItem { get; }

	private UnequipToInventoryResult(
		bool succeeded,
		UnequipToInventoryFailureReason failureReason,
		EquippedItemState unequippedItem)
	{
		Succeeded = succeeded;
		FailureReason = failureReason;
		UnequippedItem = unequippedItem;
	}

	public static UnequipToInventoryResult Success(EquippedItemState unequippedItem)
	{
		return new UnequipToInventoryResult(true, UnequipToInventoryFailureReason.None, unequippedItem);
	}

	public static UnequipToInventoryResult Fail(UnequipToInventoryFailureReason failureReason)
	{
		return new UnequipToInventoryResult(false, failureReason, null);
	}
}
