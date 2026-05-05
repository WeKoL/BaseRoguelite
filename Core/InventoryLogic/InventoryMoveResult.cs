public sealed class InventoryMoveResult
{
	public bool Succeeded { get; }
	public string Reason { get; }
	public int MovedAmount { get; }
	public InventoryMoveResult(bool succeeded, string reason, int movedAmount)
	{
		Succeeded = succeeded; Reason = reason ?? string.Empty; MovedAmount = movedAmount < 0 ? 0 : movedAmount;
	}
	public static InventoryMoveResult Success(int movedAmount) => new(true, string.Empty, movedAmount);
	public static InventoryMoveResult Fail(string reason) => new(false, reason, 0);
}
