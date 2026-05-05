using Godot;
using System.Collections.Generic;

public partial class BaseStorageState : RefCounted
{
	private readonly StorageState _storage = new();
	private readonly Dictionary<string, ItemData> _itemLookup = new();

	public IReadOnlyList<ItemEntry> Entries => BuildEntries();

	public StorageState GetLogicState()
	{
		return _storage;
	}

	public void Clear()
	{
		_storage.Clear();
	}

	public void RememberItem(ItemData item)
	{
		if (item == null || string.IsNullOrWhiteSpace(item.Id))
			return;

		_itemLookup[item.Id] = item;
	}

	public void AddItem(ItemData item, int amount)
	{
		if (item == null || amount <= 0)
			return;

		RememberItem(item);
		_storage.AddItem(item.Id, amount, Mathf.Max(1, item.MaxStackSize));
	}

	public int GetTotalAmount(string itemId)
	{
		return _storage.GetTotalAmount(itemId);
	}

	public ItemData GetItemData(string itemId)
	{
		return ResolveItem(itemId, 99);
	}

	public int RemoveItem(string itemId, int amount)
	{
		return _storage.RemoveItem(itemId, amount);
	}

	public string GetDisplayName(string itemId)
	{
		return ResolveItem(itemId, 99).DisplayName;
	}

	private IReadOnlyList<ItemEntry> BuildEntries()
	{
		var result = new List<ItemEntry>(_storage.Entries.Count);

		foreach (StorageEntryState entry in _storage.Entries)
		{
			if (entry == null || string.IsNullOrWhiteSpace(entry.ItemId) || entry.Amount <= 0)
				continue;

			ItemData item = ResolveItem(entry.ItemId, entry.MaxStackSize);
			result.Add(new ItemEntry(item, entry.Amount));
		}

		return result;
	}

	private ItemData ResolveItem(string itemId, int maxStackSize)
	{
		if (_itemLookup.TryGetValue(itemId, out ItemData item))
			return item;

		return new ItemData
		{
			Id = itemId,
			DisplayName = itemId,
			Description = "",
			Category = ItemCategory.Material,
			Rarity = ItemRarity.Common,
			MaxStackSize = maxStackSize <= 0 ? 99 : maxStackSize,
			Weight = 1.0f,
			UsageHint = "",
			Icon = null
		};
	}
}
