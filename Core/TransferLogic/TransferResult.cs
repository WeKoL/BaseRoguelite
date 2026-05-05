public sealed class TransferResult
{
	public int RequestedAmount { get; }
	public int MovedAmount { get; }

	public int RemainingAmount => RequestedAmount - MovedAmount;
	public bool MovedAnything => MovedAmount > 0;
	public bool FullyMoved => MovedAmount == RequestedAmount;

	public TransferResult(int requestedAmount, int movedAmount)
	{
		RequestedAmount = requestedAmount < 0 ? 0 : requestedAmount;
		MovedAmount = movedAmount < 0 ? 0 : movedAmount;
	}
}
