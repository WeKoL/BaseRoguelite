public sealed class GatherResult
{
	public bool Succeeded { get; }
	public string ItemId { get; }
	public int Amount { get; }
	public bool Depleted { get; }
	public string Reason { get; }

	private GatherResult(bool succeeded, string itemId, int amount, bool depleted, string reason)
	{
		Succeeded = succeeded;
		ItemId = itemId ?? string.Empty;
		Amount = amount;
		Depleted = depleted;
		Reason = reason ?? string.Empty;
	}

	public static GatherResult Success(string itemId, int amount, bool depleted)
	{
		return new GatherResult(true, itemId, amount, depleted, string.Empty);
	}

	public static GatherResult Fail(string reason = "")
	{
		return new GatherResult(false, string.Empty, 0, false, reason);
	}
}
