using System.Collections.Generic;

public sealed class SafeZoneUnloadResult
{
	private readonly List<SafeZoneUnloadEntry> _entries;

	public IReadOnlyList<SafeZoneUnloadEntry> Entries => _entries;
	public int ExpectedMovedAmount { get; }
	public int TotalMovedAmount { get; }
	public bool MovedAnything => TotalMovedAmount > 0;
	public bool FullyUnloaded => TotalMovedAmount == ExpectedMovedAmount;

	public SafeZoneUnloadResult(IEnumerable<SafeZoneUnloadEntry> entries)
	{
		_entries = entries == null ? new List<SafeZoneUnloadEntry>() : new List<SafeZoneUnloadEntry>(entries);

		int expectedTotal = 0;
		int movedTotal = 0;

		foreach (SafeZoneUnloadEntry entry in _entries)
		{
			if (entry == null)
				continue;

			expectedTotal += entry.ExpectedAmount;
			movedTotal += entry.MovedAmount;
		}

		ExpectedMovedAmount = expectedTotal;
		TotalMovedAmount = movedTotal;
	}
}
