using System;
using System.Collections.Generic;
using System.Linq;

public sealed class HarvestTarget031
{
	public string Id { get; }
	public int Danger { get; }
	public int ExpectedValue { get; }
	public float Distance { get; }
	public float Score => ExpectedValue - Danger * 2 - Distance * 0.1f;

	public HarvestTarget031(string id, int danger, int expectedValue, float distance)
	{
		Id = id ?? string.Empty;
		Danger = Math.Max(0, danger);
		ExpectedValue = Math.Max(0, expectedValue);
		Distance = Math.Max(0, distance);
	}
}

public static class HarvestRoutePlanner031
{
	public static IReadOnlyList<HarvestTarget031> PickRoute(IEnumerable<HarvestTarget031> targets, int maxDanger, int maxTargets)
	{
		return (targets ?? Array.Empty<HarvestTarget031>())
			.Where(x => x.Danger <= maxDanger)
			.OrderByDescending(x => x.Score)
			.Take(Math.Max(0, maxTargets))
			.ToList();
	}
}

public sealed class ResourceDepletionBalancer031
{
	public int BaseRespawnSeconds { get; }
	public ResourceDepletionBalancer031(int baseRespawnSeconds = 120)
	{
		BaseRespawnSeconds = Math.Max(1, baseRespawnSeconds);
	}

	public int GetRespawnSeconds(int nodeTier, int timesHarvested)
	{
		return BaseRespawnSeconds + Math.Max(0, nodeTier - 1) * 45 + Math.Max(0, timesHarvested) * 20;
	}
}
