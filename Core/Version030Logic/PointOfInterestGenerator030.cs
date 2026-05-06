using System;
using System.Collections.Generic;

public sealed class PointOfInterest030
{
	public string Id { get; }
	public string DisplayName { get; }
	public int DangerLevel { get; }
	public string RewardHint { get; }
	public PointOfInterest030(string id, string displayName, int dangerLevel, string rewardHint)
	{
		Id = id ?? string.Empty;
		DisplayName = string.IsNullOrWhiteSpace(displayName) ? Id : displayName;
		DangerLevel = Math.Max(0, dangerLevel);
		RewardHint = rewardHint ?? string.Empty;
	}
}

public static class PointOfInterestGenerator030
{
	public static IReadOnlyList<PointOfInterest030> GenerateForDanger(int dangerLevel)
	{
		int d = Math.Max(1, dangerLevel);
		List<PointOfInterest030> points = new()
		{
			new PointOfInterest030("abandoned_cache", "Заброшенный тайник", d, "еда/вода"),
			new PointOfInterest030("scrap_car", "Разбитая машина", d + 1, "металл/детали")
		};
		if (d >= 3) points.Add(new PointOfInterest030("military_crate", "Военный ящик", d + 2, "боеприпасы/броня"));
		if (d >= 5) points.Add(new PointOfInterest030("old_lab", "Старая лаборатория", d + 3, "редкие компоненты"));
		return points;
	}
}
