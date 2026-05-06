using System;
using System.Collections.Generic;
using System.Linq;

public sealed class HudModel040
{
	public string HpText { get; }
	public string NeedsText { get; }
	public string WeightText { get; }
	public string ZoneText { get; }
	public string Warning { get; }

	public HudModel040(string hpText, string needsText, string weightText, string zoneText, string warning)
	{
		HpText = hpText ?? string.Empty;
		NeedsText = needsText ?? string.Empty;
		WeightText = weightText ?? string.Empty;
		ZoneText = zoneText ?? string.Empty;
		Warning = warning ?? string.Empty;
	}
}

public sealed class HudPresenter040
{
	public HudModel040 Build(PlayerVitals040 player, InventoryCore040 inventory, WorldZone040 zone)
	{
		string warning = player.IsAlive ? string.Empty : "Игрок погиб";
		if (player.Water < 20) warning = "Мало воды";
		else if (player.Food < 20) warning = "Мало еды";
		else if (player.Stamina < 15) warning = "Нет выносливости";
		return new HudModel040($"HP {player.Hp}/{player.MaxHp}", $"Еда {player.Food} / Вода {player.Water} / Стамина {player.Stamina}", $"Вес {inventory.Weight:0.0}/{inventory.MaxWeight:0.0}", $"Зона: {zone}", warning);
	}
}

public enum MenuAction040
{
	Use,
	Equip,
	Unequip,
	Drop,
	MoveToStorage,
	Inspect,
	Repair,
	Split
}

public sealed class MenuActionProvider040
{
	private readonly ItemRegistry040 _items;
	public MenuActionProvider040(ItemRegistry040 items) { _items = items ?? throw new ArgumentNullException(nameof(items)); }
	public IReadOnlyList<MenuAction040> ForInventoryItem(string itemId, bool nearBase)
	{
		if (!_items.TryGet(itemId, out ItemDefinition040 item)) return Array.Empty<MenuAction040>();
		List<MenuAction040> actions = new();
		if (item.IsConsumable) actions.Add(MenuAction040.Use);
		if (item.IsEquipment) actions.Add(MenuAction040.Equip);
		if (item.IsStackable) actions.Add(MenuAction040.Split);
		if (nearBase) actions.Add(MenuAction040.MoveToStorage);
		actions.Add(MenuAction040.Drop);
		actions.Add(MenuAction040.Inspect);
		return actions;
	}

	public IReadOnlyList<MenuAction040> ForEquippedItem(string itemId, bool damaged)
	{
		List<MenuAction040> actions = new() { MenuAction040.Unequip };
		if (damaged) actions.Add(MenuAction040.Repair);
		actions.Add(MenuAction040.Inspect);
		return actions;
	}
}

public sealed class TooltipCore040
{
	private readonly ItemRegistry040 _items;
	public TooltipCore040(ItemRegistry040 items) { _items = items ?? throw new ArgumentNullException(nameof(items)); }
	public string BuildItemTooltip(string itemId)
	{
		if (!_items.TryGet(itemId, out ItemDefinition040 item)) return "Неизвестный предмет";
		List<string> parts = new() { item.Name, item.Category.ToString(), $"Вес {item.Weight:0.0}" };
		if (item.Armor > 0) parts.Add($"Броня +{item.Armor}");
		if (item.Damage > 0) parts.Add($"Урон +{item.Damage}");
		if (item.HealAmount > 0) parts.Add($"Лечение +{item.HealAmount}");
		return string.Join(" | ", parts);
	}
}
