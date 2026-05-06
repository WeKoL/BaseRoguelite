using System;
using System.Collections.Generic;
using System.Linq;

public enum BaseFacility040
{
	None,
	Storage,
	Workbench,
	Medbay,
	Generator,
	Walls,
	Radio
}

public sealed class BaseCore040
{
	private readonly Dictionary<BaseFacility040, int> _levels = new();
	public int Day { get; private set; } = 1;
	public int Energy { get; private set; }
	public int Defense { get; private set; }
	public int Comfort { get; private set; }
	public IReadOnlyDictionary<BaseFacility040, int> Levels => _levels;

	public int BaseLevel => _levels.Values.DefaultIfEmpty(0).Sum();
	public void SetFacility(BaseFacility040 facility, int level) { if (facility != BaseFacility040.None) _levels[facility] = Math.Max(0, level); RecalculateDerivedStats(); }
	public int GetLevel(BaseFacility040 facility) => _levels.TryGetValue(facility, out int level) ? level : 0;
	public bool HasFacility(BaseFacility040 facility, int requiredLevel) => facility == BaseFacility040.None || GetLevel(facility) >= requiredLevel;
	public void NextDay() => Day++;

	private void RecalculateDerivedStats()
	{
		Energy = GetLevel(BaseFacility040.Generator) * 10;
		Defense = GetLevel(BaseFacility040.Walls) * 8;
		Comfort = GetLevel(BaseFacility040.Storage) * 2 + GetLevel(BaseFacility040.Medbay) * 4 + GetLevel(BaseFacility040.Generator) * 2 + GetLevel(BaseFacility040.Radio) * 3;
	}
}

public sealed class BaseUpgrade040
{
	public BaseFacility040 Facility { get; }
	public int TargetLevel { get; }
	public IReadOnlyDictionary<string, int> Cost { get; }
	public BaseFacility040 RequiredFacility { get; }
	public int RequiredFacilityLevel { get; }

	public BaseUpgrade040(BaseFacility040 facility, int targetLevel, IReadOnlyDictionary<string, int> cost, BaseFacility040 requiredFacility = BaseFacility040.None, int requiredFacilityLevel = 0)
	{
		Facility = facility;
		TargetLevel = Math.Max(1, targetLevel);
		Cost = cost ?? new Dictionary<string, int>();
		RequiredFacility = requiredFacility;
		RequiredFacilityLevel = Math.Max(0, requiredFacilityLevel);
	}
}

public sealed class BaseUpgradeCore040
{
	public UpgradePreview040 Preview(BaseCore040 baseCore, StorageCore040 storage, BaseUpgrade040 upgrade)
	{
		if (baseCore == null || storage == null || upgrade == null) return UpgradePreview040.Blocked("missing_state");
		if (baseCore.GetLevel(upgrade.Facility) >= upgrade.TargetLevel) return UpgradePreview040.Blocked("already_built");
		if (!baseCore.HasFacility(upgrade.RequiredFacility, upgrade.RequiredFacilityLevel)) return UpgradePreview040.Blocked("dependency_missing");
		var missing = upgrade.Cost.Where(x => storage.Get(x.Key) < x.Value).ToDictionary(x => x.Key, x => x.Value - storage.Get(x.Key));
		return missing.Count == 0 ? UpgradePreview040.Ready() : UpgradePreview040.WithMissingResources(missing);
	}

	public UpgradePreview040 Build(BaseCore040 baseCore, StorageCore040 storage, BaseUpgrade040 upgrade)
	{
		UpgradePreview040 preview = Preview(baseCore, storage, upgrade);
		if (!preview.CanBuild) return preview;
		foreach (var cost in upgrade.Cost)
			storage.Remove(cost.Key, cost.Value, allowReserved: true);
		baseCore.SetFacility(upgrade.Facility, upgrade.TargetLevel);
		return preview;
	}
}

public sealed class UpgradePreview040
{
	public bool CanBuild { get; }
	public string Blocker { get; }
	public IReadOnlyDictionary<string, int> Missing { get; }
	private UpgradePreview040(bool canBuild, string blocker, IReadOnlyDictionary<string, int> missing) { CanBuild = canBuild; Blocker = blocker ?? string.Empty; Missing = missing ?? new Dictionary<string, int>(); }
	public static UpgradePreview040 Ready() => new(true, string.Empty, new Dictionary<string, int>());
	public static UpgradePreview040 Blocked(string blocker) => new(false, blocker, new Dictionary<string, int>());
	public static UpgradePreview040 WithMissingResources(IReadOnlyDictionary<string, int> missing) => new(false, "missing_resources", missing);
}

public sealed class BaseRaidCore040
{
	public RaidResult040 Resolve(int threat, BaseCore040 baseCore)
	{
		int defense = baseCore?.Defense ?? 0;
		int damage = Math.Max(0, threat - defense);
		return new RaidResult040(damage == 0, damage, defense);
	}
}

public sealed class RaidResult040
{
	public bool Repelled { get; }
	public int BaseDamage { get; }
	public int DefenseUsed { get; }
	public RaidResult040(bool repelled, int baseDamage, int defenseUsed) { Repelled = repelled; BaseDamage = baseDamage; DefenseUsed = defenseUsed; }
}
