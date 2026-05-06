using System;
using System.Collections.Generic;
using System.Linq;

public sealed class InventoryCore040
{
	private readonly List<ItemStack040> _slots;
	private readonly ItemRegistry040 _items;

	public IReadOnlyList<ItemStack040> Slots => _slots;
	public int MaxSlots { get; }
	public float MaxWeight { get; private set; }

	public InventoryCore040(ItemRegistry040 items, int maxSlots = 24, float maxWeight = 50f)
	{
		_items = items ?? throw new ArgumentNullException(nameof(items));
		MaxSlots = Math.Max(1, maxSlots);
		MaxWeight = Math.Max(1f, maxWeight);
		_slots = Enumerable.Range(0, MaxSlots).Select(_ => ItemStack040.Empty()).ToList();
	}

	public void SetMaxWeight(float value) => MaxWeight = Math.Max(1f, value);
	public int Count(string itemId) => _slots.Where(x => !x.IsEmpty && x.ItemId == itemId).Sum(x => x.Amount);
	public float Weight => _slots.Where(x => !x.IsEmpty).Sum(x => _items.Get(x.ItemId).Weight * x.Amount);
	public int FreeSlots => _slots.Count(x => x.IsEmpty);

	public bool CanAdd(string itemId, int amount)
	{
		if (!_items.TryGet(itemId, out ItemDefinition040 item) || amount <= 0) return false;
		if (Weight + item.Weight * amount > MaxWeight + 0.0001f) return false;
		int remaining = amount;
		foreach (ItemStack040 slot in _slots.Where(x => !x.IsEmpty && x.ItemId == itemId))
		{
			remaining -= Math.Max(0, item.MaxStack - slot.Amount);
			if (remaining <= 0) return true;
		}
		int neededNewSlots = (int)Math.Ceiling(remaining / (double)item.MaxStack);
		return neededNewSlots <= FreeSlots;
	}

	public int Add(string itemId, int amount)
	{
		if (!_items.TryGet(itemId, out ItemDefinition040 item) || amount <= 0) return 0;
		int remaining = amount;
		int added = 0;

		foreach (ItemStack040 slot in _slots.Where(x => !x.IsEmpty && x.ItemId == itemId))
		{
			int capacity = item.MaxStack - slot.Amount;
			int byWeight = GetMaxByWeight(item);
			int toAdd = Math.Min(Math.Min(capacity, remaining), byWeight);
			if (toAdd <= 0) break;
			slot.Add(toAdd);
			remaining -= toAdd;
			added += toAdd;
			if (remaining <= 0) return added;
		}

		foreach (ItemStack040 slot in _slots.Where(x => x.IsEmpty))
		{
			int byWeight = GetMaxByWeight(item);
			int toAdd = Math.Min(Math.Min(item.MaxStack, remaining), byWeight);
			if (toAdd <= 0) break;
			int durability = item.Durability > 0 ? item.Durability : 0;
			ReplaceSlot(_slots.IndexOf(slot), new ItemStack040(itemId, toAdd, durability));
			remaining -= toAdd;
			added += toAdd;
			if (remaining <= 0) break;
		}

		return added;
	}

	public int Remove(string itemId, int amount)
	{
		int remaining = Math.Max(0, amount);
		int removed = 0;
		for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
		{
			if (_slots[i].IsEmpty || _slots[i].ItemId != itemId) continue;
			removed += _slots[i].Remove(remaining);
			remaining = amount - removed;
		}
		return removed;
	}

	public bool Move(int from, int to)
	{
		if (!IsValidIndex(from) || !IsValidIndex(to) || from == to) return false;
		ItemStack040 a = _slots[from];
		ItemStack040 b = _slots[to];
		if (a.IsEmpty) return false;
		if (!b.IsEmpty && a.ItemId == b.ItemId)
		{
			ItemDefinition040 item = _items.Get(a.ItemId);
			int canMerge = Math.Max(0, item.MaxStack - b.Amount);
			int moved = a.Remove(canMerge);
			b.Add(moved);
			return moved > 0;
		}
		_slots[from] = b;
		_slots[to] = a;
		return true;
	}

	public bool Split(int index, int amount)
	{
		if (!IsValidIndex(index) || amount <= 0) return false;
		ItemStack040 source = _slots[index];
		if (source.IsEmpty || source.Amount <= amount) return false;
		int empty = _slots.FindIndex(x => x.IsEmpty);
		if (empty < 0) return false;
		source.Remove(amount);
		_slots[empty] = new ItemStack040(source.ItemId, amount, source.Durability);
		return true;
	}

	public ItemStack040 TakeFromSlot(int index)
	{
		if (!IsValidIndex(index)) return ItemStack040.Empty();
		ItemStack040 taken = _slots[index];
		_slots[index] = ItemStack040.Empty();
		return taken;
	}

	public bool InsertStack(ItemStack040 stack)
	{
		if (stack == null || stack.IsEmpty) return false;
		int added = Add(stack.ItemId, stack.Amount);
		return added == stack.Amount;
	}

	public void SortByCategoryThenName()
	{
		var nonEmpty = _slots.Where(x => !x.IsEmpty)
			.OrderBy(x => _items.Get(x.ItemId).Category)
			.ThenBy(x => _items.Get(x.ItemId).Name, StringComparer.OrdinalIgnoreCase)
			.ThenByDescending(x => x.Amount)
			.ToList();
		for (int i = 0; i < _slots.Count; i++)
			_slots[i] = i < nonEmpty.Count ? nonEmpty[i] : ItemStack040.Empty();
	}

	private int GetMaxByWeight(ItemDefinition040 item)
	{
		float free = MaxWeight - Weight;
		return Math.Max(0, (int)Math.Floor(free / item.Weight));
	}

	private void ReplaceSlot(int index, ItemStack040 stack) => _slots[index] = stack;
	private bool IsValidIndex(int index) => index >= 0 && index < _slots.Count;
}

public sealed class EquipmentCore040
{
	private readonly Dictionary<EquipmentSlot040, ItemStack040> _equipped = new();
	private readonly ItemRegistry040 _items;

	public EquipmentCore040(ItemRegistry040 items)
	{
		_items = items ?? throw new ArgumentNullException(nameof(items));
	}

	public ItemStack040 Get(EquipmentSlot040 slot) => _equipped.TryGetValue(slot, out ItemStack040 item) ? item : ItemStack040.Empty();
	public int Armor => _equipped.Values.Where(x => !x.IsEmpty).Sum(x => _items.Get(x.ItemId).Armor);
	public int Damage => _equipped.Values.Where(x => !x.IsEmpty).Sum(x => _items.Get(x.ItemId).Damage);
	public bool HasWeapon => !Get(EquipmentSlot040.Weapon).IsEmpty;

	public EquipResult040 EquipFromInventory(InventoryCore040 inventory, int slotIndex)
	{
		ItemStack040 stack = inventory.TakeFromSlot(slotIndex);
		if (stack.IsEmpty) return EquipResult040.Fail("empty_slot");
		ItemDefinition040 item = _items.Get(stack.ItemId);
		if (!item.IsEquipment)
		{
			inventory.InsertStack(stack);
			return EquipResult040.Fail("not_equipment");
		}

		ItemStack040 previous = Get(item.Slot);
		if (!previous.IsEmpty && !inventory.CanAdd(previous.ItemId, previous.Amount))
		{
			inventory.InsertStack(stack);
			return EquipResult040.Fail("inventory_full_for_previous_item");
		}

		_equipped[item.Slot] = stack;
		if (!previous.IsEmpty) inventory.InsertStack(previous);
		return EquipResult040.Success(previous);
	}

	public bool UnequipToInventory(InventoryCore040 inventory, EquipmentSlot040 slot)
	{
		ItemStack040 current = Get(slot);
		if (current.IsEmpty || !inventory.CanAdd(current.ItemId, current.Amount)) return false;
		inventory.InsertStack(current);
		_equipped.Remove(slot);
		return true;
	}

	public bool Repair(EquipmentSlot040 slot, int value)
	{
		ItemStack040 current = Get(slot);
		if (current.IsEmpty) return false;
		ItemDefinition040 item = _items.Get(current.ItemId);
		if (item.Durability <= 0) return false;
		current.Repair(value, item.Durability);
		return true;
	}
}

public sealed class EquipResult040
{
	public bool Succeeded { get; }
	public string Error { get; }
	public ItemStack040 PreviousItem { get; }

	private EquipResult040(bool succeeded, string error, ItemStack040 previousItem)
	{
		Succeeded = succeeded;
		Error = error ?? string.Empty;
		PreviousItem = previousItem ?? ItemStack040.Empty();
	}

	public static EquipResult040 Success(ItemStack040 previous) => new(true, string.Empty, previous);
	public static EquipResult040 Fail(string error) => new(false, error, ItemStack040.Empty());
}

public sealed class HotbarCore040
{
	private readonly Dictionary<int, string> _bindings = new();
	public void Bind(int slot, string itemId) { if (slot >= 1 && slot <= 9) _bindings[slot] = itemId ?? string.Empty; }
	public string GetBinding(int slot) => _bindings.TryGetValue(slot, out string itemId) ? itemId : string.Empty;
	public bool CanUse(int slot, InventoryCore040 inventory) => !string.IsNullOrWhiteSpace(GetBinding(slot)) && inventory.Count(GetBinding(slot)) > 0;
}
