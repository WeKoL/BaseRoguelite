using System; using System.Collections.Generic;
public sealed class LootTable
{
	private readonly List<LootTableEntry> _entries = new(); public IReadOnlyList<LootTableEntry> Entries => _entries;
	public void Add(LootTableEntry entry) { if (entry != null && !string.IsNullOrWhiteSpace(entry.ItemId)) _entries.Add(entry); }
	public LootTableEntry Roll(Random random) { if (_entries.Count == 0) return null; random ??= new Random(); int totalWeight = 0; foreach (LootTableEntry e in _entries) totalWeight += e.Weight; int roll = random.Next(1, totalWeight + 1); int cursor = 0; foreach (LootTableEntry e in _entries) { cursor += e.Weight; if (roll <= cursor) return e; } return _entries[^1]; }
	public int RollAmount(LootTableEntry entry, Random random) { if (entry == null) return 0; random ??= new Random(); return random.Next(entry.MinAmount, entry.MaxAmount + 1); }
}
