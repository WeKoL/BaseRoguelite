using System;
using System.Collections.Generic;

public sealed class StorageWithdrawalRequest
{
	public string ItemId { get; }
	public int DesiredAmount { get; }
	public int AvailableAmount { get; }
	public int PlannedAmount { get; }
	public int MissingAmount => Math.Max(0, DesiredAmount - PlannedAmount);
	public bool IsSatisfied => MissingAmount == 0;

	public StorageWithdrawalRequest(string itemId, int desiredAmount, int availableAmount, int plannedAmount)
	{
		ItemId = itemId ?? string.Empty;
		DesiredAmount = Math.Max(0, desiredAmount);
		AvailableAmount = Math.Max(0, availableAmount);
		PlannedAmount = Math.Max(0, plannedAmount);
	}
}

public static class StorageWithdrawalPlanner
{
	public static IReadOnlyList<StorageWithdrawalRequest> Plan(StorageState storage, IEnumerable<CraftIngredient> requestedItems)
	{
		var result = new List<StorageWithdrawalRequest>();
		if (storage == null || requestedItems == null) return result;

		foreach (CraftIngredient request in requestedItems)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.ItemId) || request.Amount <= 0) continue;
			int available = storage.GetTotalAmount(request.ItemId);
			int planned = Math.Min(available, request.Amount);
			result.Add(new StorageWithdrawalRequest(request.ItemId, request.Amount, available, planned));
		}

		return result;
	}
}
