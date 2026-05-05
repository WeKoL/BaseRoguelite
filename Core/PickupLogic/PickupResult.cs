public sealed class PickupResult
{
	public string ItemId { get; }
	public int RequestedAmount { get; }
	public int PickedAmount { get; }
	public PickupFailureReason FailureReason { get; }

	public bool Succeeded => FailureReason == PickupFailureReason.None && PickedAmount == RequestedAmount && RequestedAmount > 0;
	public bool PickedAnything => PickedAmount > 0;
	public int RemainingAmount => RequestedAmount - PickedAmount;

	public PickupResult(string itemId, int requestedAmount, int pickedAmount, PickupFailureReason failureReason)
	{
		ItemId = itemId ?? string.Empty;
		RequestedAmount = requestedAmount < 0 ? 0 : requestedAmount;
		PickedAmount = pickedAmount < 0 ? 0 : pickedAmount;
		FailureReason = failureReason;
	}
}
