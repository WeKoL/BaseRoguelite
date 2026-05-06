using System;
using System.Collections.Generic;
using System.Linq;

public sealed class StorageCore040
{
	private readonly Dictionary<string, int> _amounts = new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _reservedItems = new(StringComparer.OrdinalIgnoreCase);

	public int MaxKinds { get; private set; }
	public int MaxTotalAmount { get; private set; }
	public IReadOnlyDictionary<string, int> Amounts => _amounts;

	public StorageCore040(int maxKinds = 80, int maxTotalAmount = 3000)
	{
		MaxKinds = Math.Max(1, maxKinds);
		MaxTotalAmount = Math.Max(1, maxTotalAmount);
	}

	public int TotalAmount => _amounts.Values.Sum();
	public int FreeAmount => Math.Max(0, MaxTotalAmount - TotalAmount);
	public bool IsReserved(string itemId) => _reservedItems.Contains(itemId ?? string.Empty);
	public void ReserveForCrafting(string itemId) { if (!string.IsNullOrWhiteSpace(itemId)) _reservedItems.Add(itemId); }
	public void ClearReservation(string itemId) => _reservedItems.Remove(itemId ?? string.Empty);
	public void UpgradeCapacity(int extraKinds, int extraTotal) { MaxKinds += Math.Max(0, extraKinds); MaxTotalAmount += Math.Max(0, extraTotal); }

	public bool CanAdd(string itemId, int amount)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;
		if (!_amounts.ContainsKey(itemId) && _amounts.Count >= MaxKinds) return false;
		return TotalAmount + amount <= MaxTotalAmount;
	}

	public int Add(string itemId, int amount)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return 0;
		if (!_amounts.ContainsKey(itemId) && _amounts.Count >= MaxKinds) return 0;
		int accepted = Math.Min(amount, FreeAmount);
		if (accepted <= 0) return 0;
		_amounts[itemId] = Get(itemId) + accepted;
		return accepted;
	}

	public int Remove(string itemId, int amount, bool allowReserved = false)
	{
		if (IsReserved(itemId) && !allowReserved) return 0;
		int available = Get(itemId);
		int removed = Math.Min(available, Math.Max(0, amount));
		if (removed <= 0) return 0;
		int left = available - removed;
		if (left <= 0) _amounts.Remove(itemId);
		else _amounts[itemId] = left;
		return removed;
	}

	public int Get(string itemId) => _amounts.TryGetValue(itemId ?? string.Empty, out int amount) ? amount : 0;
	public IEnumerable<string> Search(string fragment) => _amounts.Keys.Where(x => x.Contains(fragment ?? string.Empty, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x);

	public StorageAudit040 Audit()
	{
		IReadOnlyList<string> lowBasic = new[] { "wood", "stone", "metal", "food", "water" }.Where(x => Get(x) < 3).ToArray();
		return new StorageAudit040(_amounts.Count, TotalAmount, FreeAmount, lowBasic, _reservedItems.ToArray());
	}
}

public sealed class StorageAudit040
{
	public int KindCount { get; }
	public int TotalAmount { get; }
	public int FreeAmount { get; }
	public IReadOnlyList<string> LowBasicItems { get; }
	public IReadOnlyList<string> ReservedItems { get; }
	public bool HasCriticalShortage => LowBasicItems.Count > 0;

	public StorageAudit040(int kindCount, int totalAmount, int freeAmount, IReadOnlyList<string> lowBasicItems, IReadOnlyList<string> reservedItems)
	{
		KindCount = kindCount;
		TotalAmount = totalAmount;
		FreeAmount = freeAmount;
		LowBasicItems = lowBasicItems ?? Array.Empty<string>();
		ReservedItems = reservedItems ?? Array.Empty<string>();
	}
}

public sealed class AutoUnloadCore040
{
	private readonly HashSet<ItemCategory040> _categories = new();
	private readonly ItemRegistry040 _items;

	public AutoUnloadCore040(ItemRegistry040 items)
	{
		_items = items ?? throw new ArgumentNullException(nameof(items));
	}

	public void EnableCategory(ItemCategory040 category) => _categories.Add(category);
	public bool ShouldUnload(string itemId) => _items.TryGet(itemId, out ItemDefinition040 item) && _categories.Contains(item.Category);

	public int UnloadMatching(InventoryCore040 inventory, StorageCore040 storage)
	{
		int moved = 0;
		foreach (ItemStack040 stack in inventory.Slots.Where(x => !x.IsEmpty).Select(x => x.Clone()).ToArray())
		{
			if (!ShouldUnload(stack.ItemId)) continue;
			int accepted = storage.Add(stack.ItemId, stack.Amount);
			if (accepted <= 0) continue;
			inventory.Remove(stack.ItemId, accepted);
			moved += accepted;
		}
		return moved;
	}
}
