using System;
using System.Collections.Generic;
using System.Linq;

public sealed class BranchCompletionReport030
{
	private readonly List<BranchCompletionItem030> _items = new();
	public IReadOnlyList<BranchCompletionItem030> Items => _items;
	public int BranchCount => _items.Count;
	public int IntegratedCount => _items.Count(x => (int)x.Status >= (int)BranchCompletionStatus030.Integrated);
	public int TotalTests => _items.Sum(x => x.TestCount);
	public bool CoversAllFourteenBranches => _items.Select(x => x.BranchNumber).Distinct().Count() >= 14;
	public bool HasPlayableHooksForMostBranches => _items.Count(x => x.HasVisibleGameplayHook) >= Math.Min(10, _items.Count);
	public bool IsReleaseCandidate => CoversAllFourteenBranches && IntegratedCount >= 10 && TotalTests >= 28;

	public BranchCompletionReport030(IEnumerable<BranchCompletionItem030> items)
	{
		if (items != null)
			_items.AddRange(items.Where(x => x != null));
	}

	public IReadOnlyList<BranchCompletionItem030> GetBranchesBelow(BranchCompletionStatus030 status)
	{
		return _items.Where(x => (int)x.Status < (int)status).ToList();
	}
}
