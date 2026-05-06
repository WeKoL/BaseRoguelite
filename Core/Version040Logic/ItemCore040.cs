using System;
using System.Collections.Generic;
using System.Linq;

public enum ItemCategory040
{
	Material,
	Consumable,
	Equipment,
	Weapon,
	Tool,
	Quest,
	Ammo
}

public enum EquipmentSlot040
{
	None,
	Head,
	Body,
	Backpack,
	Weapon,
	Tool
}

public sealed class ItemDefinition040
{
	public string Id { get; }
	public string Name { get; }
	public ItemCategory040 Category { get; }
	public int MaxStack { get; }
	public float Weight { get; }
	public EquipmentSlot040 Slot { get; }
	public int Armor { get; }
	public int Damage { get; }
	public int HealAmount { get; }
	public int FoodAmount { get; }
	public int WaterAmount { get; }
	public int Durability { get; }

	public bool IsStackable => MaxStack > 1;
	public bool IsEquipment => Slot != EquipmentSlot040.None;
	public bool IsConsumable => HealAmount > 0 || FoodAmount > 0 || WaterAmount > 0;

	public ItemDefinition040(string id, string name, ItemCategory040 category, int maxStack, float weight, EquipmentSlot040 slot = EquipmentSlot040.None, int armor = 0, int damage = 0, int healAmount = 0, int foodAmount = 0, int waterAmount = 0, int durability = 0)
	{
		Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Item id is required.", nameof(id)) : id;
		Name = string.IsNullOrWhiteSpace(name) ? Id : name;
		Category = category;
		MaxStack = Math.Max(1, maxStack);
		Weight = Math.Max(0.01f, weight);
		Slot = slot;
		Armor = Math.Max(0, armor);
		Damage = Math.Max(0, damage);
		HealAmount = Math.Max(0, healAmount);
		FoodAmount = Math.Max(0, foodAmount);
		WaterAmount = Math.Max(0, waterAmount);
		Durability = Math.Max(0, durability);
	}
}

public sealed class ItemRegistry040
{
	private readonly Dictionary<string, ItemDefinition040> _items = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyDictionary<string, ItemDefinition040> Items => _items;

	public void Register(ItemDefinition040 item)
	{
		if (item == null) throw new ArgumentNullException(nameof(item));
		_items[item.Id] = item;
	}

	public ItemDefinition040 Get(string id)
	{
		if (!_items.TryGetValue(id ?? string.Empty, out ItemDefinition040 item))
			throw new InvalidOperationException($"Unknown item: {id}");
		return item;
	}

	public bool TryGet(string id, out ItemDefinition040 item) => _items.TryGetValue(id ?? string.Empty, out item);

	public static ItemRegistry040 CreateDefault()
	{
		ItemRegistry040 registry = new();
		registry.Register(new ItemDefinition040("wood", "Доски", ItemCategory040.Material, 99, 1f));
		registry.Register(new ItemDefinition040("stone", "Камень", ItemCategory040.Material, 99, 1f));
		registry.Register(new ItemDefinition040("metal", "Металл", ItemCategory040.Material, 99, 1f));
		registry.Register(new ItemDefinition040("cloth", "Ткань", ItemCategory040.Material, 99, 1f));
		registry.Register(new ItemDefinition040("herb", "Трава", ItemCategory040.Material, 99, 1f));
		registry.Register(new ItemDefinition040("water", "Вода", ItemCategory040.Consumable, 10, 1f, waterAmount: 25));
		registry.Register(new ItemDefinition040("food", "Консервы", ItemCategory040.Consumable, 10, 1f, foodAmount: 25));
		registry.Register(new ItemDefinition040("medkit", "Аптечка", ItemCategory040.Consumable, 5, 1f, healAmount: 35));
		registry.Register(new ItemDefinition040("repair_kit", "Ремкомплект", ItemCategory040.Consumable, 5, 1f));
		registry.Register(new ItemDefinition040("helmet", "Шлем", ItemCategory040.Equipment, 1, 3f, EquipmentSlot040.Head, armor: 2, durability: 100));
		registry.Register(new ItemDefinition040("armor", "Броня", ItemCategory040.Equipment, 1, 6f, EquipmentSlot040.Body, armor: 5, durability: 120));
		registry.Register(new ItemDefinition040("backpack", "Рюкзак", ItemCategory040.Equipment, 1, 2f, EquipmentSlot040.Backpack, durability: 80));
		registry.Register(new ItemDefinition040("knife", "Нож", ItemCategory040.Weapon, 1, 2f, EquipmentSlot040.Weapon, damage: 5, durability: 80));
		registry.Register(new ItemDefinition040("rifle", "Винтовка", ItemCategory040.Weapon, 1, 5f, EquipmentSlot040.Weapon, damage: 14, durability: 100));
		registry.Register(new ItemDefinition040("ammo_basic", "Патроны", ItemCategory040.Ammo, 60, 0.1f));
		registry.Register(new ItemDefinition040("axe", "Топор", ItemCategory040.Tool, 1, 3f, EquipmentSlot040.Tool, damage: 3, durability: 80));
		registry.Register(new ItemDefinition040("pickaxe", "Кирка", ItemCategory040.Tool, 1, 4f, EquipmentSlot040.Tool, damage: 3, durability: 80));
		return registry;
	}
}

public sealed class ItemStack040
{
	public string ItemId { get; private set; }
	public int Amount { get; private set; }
	public int Durability { get; private set; }
	public bool IsEmpty => string.IsNullOrWhiteSpace(ItemId) || Amount <= 0;

	public ItemStack040(string itemId, int amount, int durability = 0)
	{
		ItemId = itemId ?? string.Empty;
		Amount = Math.Max(0, amount);
		Durability = Math.Max(0, durability);
		if (Amount <= 0) ItemId = string.Empty;
	}

	public static ItemStack040 Empty() => new(string.Empty, 0);
	public ItemStack040 Clone() => new(ItemId, Amount, Durability);

	public void Add(int amount) => Amount += Math.Max(0, amount);

	public int Remove(int amount)
	{
		int taken = Math.Min(Amount, Math.Max(0, amount));
		Amount -= taken;
		if (Amount <= 0)
		{
			ItemId = string.Empty;
			Durability = 0;
		}
		return taken;
	}

	public void DamageDurability(int value)
	{
		if (Durability <= 0) return;
		Durability = Math.Max(0, Durability - Math.Max(0, value));
	}

	public void Repair(int value, int maxDurability)
	{
		Durability = Math.Min(Math.Max(0, maxDurability), Durability + Math.Max(0, value));
	}
}
