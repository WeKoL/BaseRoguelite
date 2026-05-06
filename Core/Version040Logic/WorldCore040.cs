using System;
using System.Collections.Generic;
using System.Linq;

public enum WorldZone040
{
	Base,
	NearField,
	Ruins,
	Factory,
	Bunker
}

public sealed class ZoneRule040
{
	public WorldZone040 Zone { get; }
	public int Danger { get; }
	public int RequiredBaseLevel { get; }
	public float LootMultiplier { get; }
	public ZoneRule040(WorldZone040 zone, int danger, int requiredBaseLevel, float lootMultiplier)
	{
		Zone = zone;
		Danger = Math.Max(0, danger);
		RequiredBaseLevel = Math.Max(0, requiredBaseLevel);
		LootMultiplier = Math.Max(0.1f, lootMultiplier);
	}
}

public sealed class WorldCore040
{
	private readonly List<ZoneRule040> _zones = new();
	private readonly List<(int Minute, string EventId)> _events = new();

	public IReadOnlyList<ZoneRule040> Zones => _zones;
	public void AddZone(ZoneRule040 rule) { if (rule != null) _zones.Add(rule); }
	public void AddEvent(int minute, string eventId) { if (!string.IsNullOrWhiteSpace(eventId)) _events.Add((Math.Max(0, minute), eventId)); }
	public IEnumerable<ZoneRule040> GetUnlockedZones(int baseLevel) => _zones.Where(x => x.RequiredBaseLevel <= baseLevel).OrderBy(x => x.Danger);
	public IEnumerable<string> GetDueEvents(int elapsedMinutes) => _events.Where(x => x.Minute <= elapsedMinutes).OrderBy(x => x.Minute).Select(x => x.EventId);

	public static WorldCore040 CreateDefault()
	{
		WorldCore040 world = new();
		world.AddZone(new ZoneRule040(WorldZone040.Base, 0, 0, 0.2f));
		world.AddZone(new ZoneRule040(WorldZone040.NearField, 1, 0, 1f));
		world.AddZone(new ZoneRule040(WorldZone040.Ruins, 2, 1, 1.4f));
		world.AddZone(new ZoneRule040(WorldZone040.Factory, 4, 2, 2f));
		world.AddZone(new ZoneRule040(WorldZone040.Bunker, 6, 3, 3f));
		world.AddEvent(5, "first_scavenge_hint");
		world.AddEvent(12, "danger_rises");
		world.AddEvent(20, "night_wave_warning");
		return world;
	}
}

public sealed class ExpeditionCore040
{
	public WorldZone040 CurrentZone { get; private set; } = WorldZone040.Base;
	public int ElapsedMinutes { get; private set; }
	public bool Extracted { get; private set; }
	public bool Failed { get; private set; }

	public void Enter(WorldZone040 zone)
	{
		CurrentZone = zone;
		Extracted = false;
		Failed = false;
		ElapsedMinutes = 0;
	}

	public ExpeditionTick040 Tick(int minutes, PlayerVitals040 vitals, ZoneRule040 rule)
	{
		ElapsedMinutes += Math.Max(0, minutes);
		if (rule != null && rule.Danger > 0 && ElapsedMinutes % 10 == 0)
			vitals.Damage(rule.Danger, 0);
		if (!vitals.IsAlive) Failed = true;
		return new ExpeditionTick040(ElapsedMinutes, CurrentZone, Failed, ElapsedMinutes >= 25 && CurrentZone != WorldZone040.Base);
	}

	public bool ExtractToBase()
	{
		if (Failed) return false;
		Extracted = true;
		CurrentZone = WorldZone040.Base;
		return true;
	}
}

public sealed class ExpeditionTick040
{
	public int ElapsedMinutes { get; }
	public WorldZone040 Zone { get; }
	public bool Failed { get; }
	public bool ShouldReturn { get; }
	public ExpeditionTick040(int elapsedMinutes, WorldZone040 zone, bool failed, bool shouldReturn) { ElapsedMinutes = elapsedMinutes; Zone = zone; Failed = failed; ShouldReturn = shouldReturn; }
}
