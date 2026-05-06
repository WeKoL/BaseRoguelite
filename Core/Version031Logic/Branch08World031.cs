using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ZoneUnlock031
{
	public string ZoneId { get; }
	public int RequiredBaseLevel { get; }
	public int RequiredDangerMastery { get; }
	public ZoneUnlock031(string zoneId, int requiredBaseLevel, int requiredDangerMastery)
	{
		ZoneId = zoneId ?? string.Empty;
		RequiredBaseLevel = Math.Max(0, requiredBaseLevel);
		RequiredDangerMastery = Math.Max(0, requiredDangerMastery);
	}
	public bool IsUnlocked(int baseLevel, int dangerMastery) => baseLevel >= RequiredBaseLevel && dangerMastery >= RequiredDangerMastery;
}

public sealed class WorldEventSchedule031
{
	private readonly List<(float Time, string EventId)> _events = new();
	public void Add(float time, string eventId)
	{
		if (!string.IsNullOrWhiteSpace(eventId))
			_events.Add((Math.Max(0, time), eventId));
	}
	public IReadOnlyList<string> GetDueEvents(float elapsedTime)
	{
		return _events.Where(x => x.Time <= elapsedTime).OrderBy(x => x.Time).Select(x => x.EventId).ToList();
	}
}

public static class ZoneProgressionPlanner031
{
	public static IReadOnlyList<string> GetUnlockedZones(IEnumerable<ZoneUnlock031> zones, int baseLevel, int dangerMastery)
	{
		return (zones ?? Array.Empty<ZoneUnlock031>()).Where(x => x.IsUnlocked(baseLevel, dangerMastery)).Select(x => x.ZoneId).ToList();
	}
}
