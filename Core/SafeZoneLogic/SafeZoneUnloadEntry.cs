public sealed class SafeZoneUnloadEntry
{
	public string ItemId { get; }
	public int ExpectedAmount { get; }
	public int MovedAmount { get; }
	public int FinalStorageAmount { get; }

	public int RemainingAmount => System.Math.Max(0, ExpectedAmount - MovedAmount);
	public bool FullyMoved => RemainingAmount == 0;

	public SafeZoneUnloadEntry(string itemId, int expectedAmount, int movedAmount, int finalStorageAmount)
	{
		ItemId = itemId ?? string.Empty;
		ExpectedAmount = expectedAmount < 0 ? 0 : expectedAmount;
		MovedAmount = movedAmount < 0 ? 0 : movedAmount;
		FinalStorageAmount = finalStorageAmount < 0 ? 0 : finalStorageAmount;
	}
}
