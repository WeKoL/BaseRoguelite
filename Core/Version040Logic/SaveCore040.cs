using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public sealed class SaveGame040
{
	public string Version { get; set; } = Version040Info.Version;
	public string SaveVersion { get; set; } = Version040Info.SaveVersion;
	public int Day { get; set; }
	public int Hp { get; set; }
	public int Food { get; set; }
	public int Water { get; set; }
	public Dictionary<string, int> Inventory { get; set; } = new();
	public Dictionary<string, int> Storage { get; set; } = new();
	public Dictionary<string, int> Facilities { get; set; } = new();
	public List<string> CompletedQuests { get; set; } = new();
}

public sealed class SaveCore040
{
	public string Serialize(SaveGame040 save)
	{
		return JsonSerializer.Serialize(save ?? new SaveGame040(), new JsonSerializerOptions { WriteIndented = true });
	}

	public SaveGame040 Deserialize(string json)
	{
		if (string.IsNullOrWhiteSpace(json)) return new SaveGame040();
		SaveGame040 save = JsonSerializer.Deserialize<SaveGame040>(json) ?? new SaveGame040();
		return Migrate(save);
	}

	public SaveGame040 Build(GameCore040 game)
	{
		SaveGame040 save = new();
		save.Day = game.Base.Day;
		save.Hp = game.Player.Hp;
		save.Food = game.Player.Food;
		save.Water = game.Player.Water;
		foreach (ItemStack040 stack in game.Inventory.Slots.Where(x => !x.IsEmpty))
			save.Inventory[stack.ItemId] = save.Inventory.TryGetValue(stack.ItemId, out int old) ? old + stack.Amount : stack.Amount;
		foreach (var item in game.Storage.Amounts)
			save.Storage[item.Key] = item.Value;
		foreach (var facility in game.Base.Levels)
			save.Facilities[facility.Key.ToString()] = facility.Value;
		foreach (string questId in game.Quests.Completed)
			save.CompletedQuests.Add(questId);
		return save;
	}

	public SaveValidation040 Validate(SaveGame040 save)
	{
		List<string> errors = new();
		if (save == null) errors.Add("save_missing");
		else
		{
			if (save.Hp < 0) errors.Add("hp_negative");
			if (save.Food < 0 || save.Water < 0) errors.Add("needs_negative");
			if (save.Inventory.Any(x => x.Value < 0) || save.Storage.Any(x => x.Value < 0)) errors.Add("negative_items");
			if (string.IsNullOrWhiteSpace(save.SaveVersion)) errors.Add("save_version_missing");
		}
		return new SaveValidation040(errors);
	}

	public SaveGame040 Migrate(SaveGame040 save)
	{
		if (save == null) return new SaveGame040();
		if (string.IsNullOrWhiteSpace(save.Version)) save.Version = Version040Info.Version;
		if (string.IsNullOrWhiteSpace(save.SaveVersion) || save.SaveVersion != Version040Info.SaveVersion) save.SaveVersion = Version040Info.SaveVersion;
		if (save.Food <= 0) save.Food = 100;
		if (save.Water <= 0) save.Water = 100;
		if (save.Hp <= 0) save.Hp = 100;
		save.Inventory ??= new Dictionary<string, int>();
		save.Storage ??= new Dictionary<string, int>();
		save.Facilities ??= new Dictionary<string, int>();
		save.CompletedQuests ??= new List<string>();
		return save;
	}
}

public sealed class SaveValidation040
{
	public IReadOnlyList<string> Errors { get; }
	public bool IsValid => Errors.Count == 0;
	public SaveValidation040(IReadOnlyList<string> errors) { Errors = errors ?? Array.Empty<string>(); }
}
