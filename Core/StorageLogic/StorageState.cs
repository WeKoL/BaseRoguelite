using System;
using System.Collections.Generic;

public sealed class StorageState
{
	private readonly List<StorageEntryState> _entries = new();
	public IReadOnlyList<StorageEntryState> Entries => _entries;
	public int MaxEntries { get; }
	public bool HasEntryLimit => MaxEntries < int.MaxValue;
	public int FreeEntrySlots => HasEntryLimit ? Math.Max(0, MaxEntries - _entries.Count) : int.MaxValue;

	public StorageState() : this(int.MaxValue) { }
	public StorageState(int maxEntries)
	{
		MaxEntries = maxEntries <= 0 ? int.MaxValue : maxEntries;
	}

	public void Clear() => _entries.Clear();
	public void AddItem(string itemId, int amount, int maxStackSize) => TryAddItem(itemId, amount, maxStackSize);

	public int TryAddItem(string itemId, int amount, int maxStackSize)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0 || maxStackSize <= 0) return 0;
		int remaining = amount;
		int addedTotal = 0;
		foreach (StorageEntryState entry in _entries)
		{
			if (entry.ItemId != itemId) continue;
			int added = entry.AddAmount(remaining);
			addedTotal += added;
			remaining -= added;
			if (remaining <= 0) return addedTotal;
		}
		while (remaining > 0 && _entries.Count < MaxEntries)
		{
			int stackAmount = Math.Min(maxStackSize, remaining);
			_entries.Add(new StorageEntryState(itemId, stackAmount, maxStackSize));
			addedTotal += stackAmount;
			remaining -= stackAmount;
		}
		return addedTotal;
	}

	public bool CanFit(string itemId, int amount, int maxStackSize)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0 || maxStackSize <= 0) return false;
		int remaining = amount;
		foreach (StorageEntryState entry in _entries)
		{
			if (entry.ItemId != itemId) continue;
			remaining -= Math.Max(0, entry.MaxStackSize - entry.Amount);
			if (remaining <= 0) return true;
		}
		if (FreeEntrySlots == int.MaxValue) return true;
		int requiredNewStacks = (int)Math.Ceiling(remaining / (double)maxStackSize);
		return requiredNewStacks <= FreeEntrySlots;
	}

	public int RemoveItem(string itemId, int amount)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return 0;
		int remaining = amount;
		int removed = 0;
		for (int i = _entries.Count - 1; i >= 0; i--)
		{
			StorageEntryState entry = _entries[i];
			if (entry.ItemId != itemId) continue;
			int take = Math.Min(entry.Amount, remaining);
			entry.RemoveAmount(take);
			removed += take;
			remaining -= take;
			if (entry.Amount <= 0) _entries.RemoveAt(i);
			if (remaining <= 0) break;
		}
		return removed;
	}

	public int GetTotalAmount(string itemId)
	{
		if (string.IsNullOrWhiteSpace(itemId)) return 0;
		int total = 0;
		foreach (StorageEntryState entry in _entries) if (entry.ItemId == itemId) total += entry.Amount;
		return total;
	}

	public int RemoveEmptyEntries()
	{
		int removed = 0;
		for (int i = _entries.Count - 1; i >= 0; i--)
		{
			if (_entries[i].Amount > 0) continue;
			_entries.RemoveAt(i);
			removed++;
		}
		return removed;
	}

	public int GetUsedEntryCount() => _entries.Count;
}
