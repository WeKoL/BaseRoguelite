using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ResourceNode040
{
	public string Id { get; }
	public string RequiredToolId { get; }
	public int Charges { get; private set; }
	public int MaxCharges { get; }
	public int RespawnSeconds { get; }
	public IReadOnlyDictionary<string, int> Loot { get; }

	public bool IsDepleted => Charges <= 0;

	public ResourceNode040(string id, IReadOnlyDictionary<string, int> loot, int charges = 3, string requiredToolId = "", int respawnSeconds = 120)
	{
		Id = id ?? string.Empty;
		Loot = loot ?? new Dictionary<string, int>();
		Charges = Math.Max(0, charges);
		MaxCharges = Math.Max(1, charges);
		RequiredToolId = requiredToolId ?? string.Empty;
		RespawnSeconds = Math.Max(1, respawnSeconds);
	}

	public IReadOnlyDictionary<string, int> Gather(string toolId)
	{
		if (IsDepleted) return new Dictionary<string, int>();
		if (!string.IsNullOrWhiteSpace(RequiredToolId) && RequiredToolId != (toolId ?? string.Empty)) return new Dictionary<string, int>();
		Charges--;
		return Loot.ToDictionary(x => x.Key, x => x.Value);
	}

	public void Regenerate(int seconds)
	{
		if (Charges >= MaxCharges) return;
		int restored = Math.Max(0, seconds) / RespawnSeconds;
		Charges = Math.Min(MaxCharges, Charges + restored);
	}
}

public sealed class GatheringCore040
{
	private readonly ItemRegistry040 _items;
	public GatheringCore040(ItemRegistry040 items) { _items = items ?? throw new ArgumentNullException(nameof(items)); }

	public GatherOutcome040 Gather(ResourceNode040 node, string toolId, InventoryCore040 inventory)
	{
		if (node == null) return GatherOutcome040.Fail("node_missing");
		if (node.IsDepleted) return GatherOutcome040.Fail("depleted");
		var loot = node.Gather(toolId);
		if (loot.Count == 0) return GatherOutcome040.Fail("tool_required");

		Dictionary<string, int> added = new();
		foreach (var pair in loot)
		{
			if (!_items.TryGet(pair.Key, out _)) continue;
			int amount = inventory.Add(pair.Key, pair.Value);
			if (amount > 0) added[pair.Key] = amount;
		}
		return added.Count == 0 ? GatherOutcome040.Fail("inventory_full") : GatherOutcome040.Success(added);
	}
}

public sealed class GatherOutcome040
{
	public bool Succeeded { get; }
	public string Error { get; }
	public IReadOnlyDictionary<string, int> Added { get; }

	private GatherOutcome040(bool succeeded, string error, IReadOnlyDictionary<string, int> added)
	{
		Succeeded = succeeded;
		Error = error ?? string.Empty;
		Added = added ?? new Dictionary<string, int>();
	}

	public static GatherOutcome040 Success(IReadOnlyDictionary<string, int> added) => new(true, string.Empty, added);
	public static GatherOutcome040 Fail(string error) => new(false, error, new Dictionary<string, int>());
}

public sealed class LootTable040
{
	private readonly List<(string ItemId, int Amount, int Weight)> _entries = new();
	public void Add(string itemId, int amount, int weight) { if (!string.IsNullOrWhiteSpace(itemId) && amount > 0 && weight > 0) _entries.Add((itemId, amount, weight)); }
	public (string ItemId, int Amount) Roll(int seed)
	{
		if (_entries.Count == 0) return (string.Empty, 0);
		int total = _entries.Sum(x => x.Weight);
		int value = Math.Abs(seed) % total;
		foreach (var entry in _entries)
		{
			if (value < entry.Weight) return (entry.ItemId, entry.Amount);
			value -= entry.Weight;
		}
		var last = _entries[_entries.Count - 1];
		return (last.ItemId, last.Amount);
	}
}
