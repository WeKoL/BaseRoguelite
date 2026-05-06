using System;
using System.Collections.Generic;
using System.Linq;

public enum StatusKind040
{
	Bleeding,
	Poisoned,
	Cold,
	Radiation,
	Rested,
	Hungry,
	Thirsty
}

public sealed class PlayerVitals040
{
	private readonly Dictionary<StatusKind040, int> _statuses = new();

	public int MaxHp { get; private set; } = 100;
	public int Hp { get; private set; } = 100;
	public int Stamina { get; private set; } = 100;
	public int Food { get; private set; } = 100;
	public int Water { get; private set; } = 100;
	public bool IsAlive => Hp > 0;
	public IReadOnlyDictionary<StatusKind040, int> Statuses => _statuses;

	public void SetMaxHp(int value)
	{
		MaxHp = Math.Max(1, value);
		Hp = Math.Min(Hp, MaxHp);
	}

	public void Damage(int amount, int armor = 0)
	{
		int damage = Math.Max(0, amount - Math.Max(0, armor));
		Hp = Math.Max(0, Hp - damage);
	}

	public void Heal(int amount) => Hp = Math.Min(MaxHp, Hp + Math.Max(0, amount));
	public bool SpendStamina(int amount) { amount = Math.Max(0, amount); if (Stamina < amount) return false; Stamina -= amount; return true; }
	public void RestoreStamina(int amount) => Stamina = Math.Min(100, Stamina + Math.Max(0, amount));
	public void Eat(int amount) => Food = Math.Min(100, Food + Math.Max(0, amount));
	public void Drink(int amount) => Water = Math.Min(100, Water + Math.Max(0, amount));
	public void AddStatus(StatusKind040 kind, int seconds) => _statuses[kind] = Math.Max(GetStatusSeconds(kind), Math.Max(0, seconds));
	public int GetStatusSeconds(StatusKind040 kind) => _statuses.TryGetValue(kind, out int seconds) ? seconds : 0;

	public SurvivalTickResult040 Tick(int seconds, bool running, bool inBase)
	{
		seconds = Math.Max(0, seconds);
		int hpBefore = Hp;
		Food = Math.Max(0, Food - seconds / (inBase ? 120 : 60));
		Water = Math.Max(0, Water - seconds / (inBase ? 90 : 45));
		if (running) SpendStamina(seconds / 2 + 1);
		else RestoreStamina(seconds / (inBase ? 3 : 5));
		if (Food <= 0) AddStatus(StatusKind040.Hungry, 30);
		if (Water <= 0) AddStatus(StatusKind040.Thirsty, 30);

		var keys = _statuses.Keys.ToArray();
		foreach (StatusKind040 key in keys)
		{
			int left = Math.Max(0, _statuses[key] - seconds);
			if (left <= 0) _statuses.Remove(key);
			else _statuses[key] = left;
		}

		if (GetStatusSeconds(StatusKind040.Bleeding) > 0) Hp = Math.Max(0, Hp - Math.Max(1, seconds / 10));
		if (GetStatusSeconds(StatusKind040.Poisoned) > 0) Hp = Math.Max(0, Hp - Math.Max(1, seconds / 15));
		if (GetStatusSeconds(StatusKind040.Hungry) > 0) Hp = Math.Max(0, Hp - 1);
		if (GetStatusSeconds(StatusKind040.Thirsty) > 0) Hp = Math.Max(0, Hp - 2);

		return new SurvivalTickResult040(hpBefore - Hp, Food, Water, Stamina, _statuses.Keys.ToArray());
	}
}

public sealed class SurvivalTickResult040
{
	public int DamageTaken { get; }
	public int Food { get; }
	public int Water { get; }
	public int Stamina { get; }
	public IReadOnlyList<StatusKind040> ActiveStatuses { get; }
	public bool HasPressure => DamageTaken > 0 || Food < 25 || Water < 25 || Stamina < 20 || ActiveStatuses.Count > 0;

	public SurvivalTickResult040(int damageTaken, int food, int water, int stamina, IReadOnlyList<StatusKind040> activeStatuses)
	{
		DamageTaken = Math.Max(0, damageTaken);
		Food = food;
		Water = water;
		Stamina = stamina;
		ActiveStatuses = activeStatuses ?? Array.Empty<StatusKind040>();
	}
}

public sealed class ConsumableUseCore040
{
	private readonly ItemRegistry040 _items;
	public ConsumableUseCore040(ItemRegistry040 items) { _items = items ?? throw new ArgumentNullException(nameof(items)); }

	public bool Use(string itemId, InventoryCore040 inventory, PlayerVitals040 vitals)
	{
		if (!_items.TryGet(itemId, out ItemDefinition040 item) || !item.IsConsumable || inventory.Count(itemId) <= 0) return false;
		inventory.Remove(itemId, 1);
		vitals.Heal(item.HealAmount);
		vitals.Eat(item.FoodAmount);
		vitals.Drink(item.WaterAmount);
		return true;
	}
}
