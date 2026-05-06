using System;
using System.Collections.Generic;
using System.Linq;

public enum StoragePolicy031
{
	KeepInInventory,
	MoveToStorage,
	ReserveForCrafting,
	TrashCandidate
}

public sealed class StoragePolicyRule031
{
	public string ItemId { get; }
	public StoragePolicy031 Policy { get; }
	public int KeepAmount { get; }

	public StoragePolicyRule031(string itemId, StoragePolicy031 policy, int keepAmount = 0)
	{
		ItemId = string.IsNullOrWhiteSpace(itemId) ? string.Empty : itemId.Trim();
		Policy = policy;
		KeepAmount = Math.Max(0, keepAmount);
	}
}

public sealed class StoragePolicyPlanner031
{
	private readonly List<StoragePolicyRule031> _rules = new();
	public IReadOnlyList<StoragePolicyRule031> Rules => _rules;

	public void AddRule(StoragePolicyRule031 rule)
	{
		if (rule != null && !string.IsNullOrWhiteSpace(rule.ItemId))
			_rules.RemoveAll(x => x.ItemId == rule.ItemId);
		if (rule != null && !string.IsNullOrWhiteSpace(rule.ItemId))
			_rules.Add(rule);
	}

	public StoragePolicy031 GetPolicy(string itemId)
	{
		return _rules.FirstOrDefault(x => x.ItemId == itemId)?.Policy ?? StoragePolicy031.MoveToStorage;
	}

	public int GetKeepAmount(string itemId)
	{
		return _rules.FirstOrDefault(x => x.ItemId == itemId)?.KeepAmount ?? 0;
	}
}

public sealed class StorageAuditReport031
{
	public int TotalStacks { get; }
	public int DuplicateItemKinds { get; }
	public int OverLimitStacks { get; }
	public bool NeedsCompaction => DuplicateItemKinds > 0 || OverLimitStacks > 0;

	public StorageAuditReport031(IEnumerable<(string ItemId, int Amount, int MaxStack)> entries)
	{
		var list = entries?.ToList() ?? new List<(string ItemId, int Amount, int MaxStack)>();
		TotalStacks = list.Count;
		DuplicateItemKinds = list.GroupBy(x => x.ItemId).Count(g => g.Count() > 1);
		OverLimitStacks = list.Count(x => x.MaxStack > 0 && x.Amount > x.MaxStack);
	}
}
