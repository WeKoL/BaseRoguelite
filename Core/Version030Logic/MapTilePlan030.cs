using System;
using System.Collections.Generic;

public sealed class MapTile030
{
	public int X { get; }
	public int Y { get; }
	public WorldZoneKind ZoneKind { get; }
	public int DangerLevel { get; }
	public MapTile030(int x, int y, WorldZoneKind zoneKind, int dangerLevel)
	{
		X = x; Y = y; ZoneKind = zoneKind; DangerLevel = Math.Max(0, dangerLevel);
	}
}

public static class MapTilePlanner030
{
	public static IReadOnlyList<MapTile030> GenerateSquareAroundBase(int radius)
	{
		int r = Math.Max(1, radius);
		List<MapTile030> tiles = new();
		for (int y = -r; y <= r; y++)
		for (int x = -r; x <= r; x++)
		{
			int distance = Math.Abs(x) + Math.Abs(y);
			WorldZoneKind kind = distance == 0 ? WorldZoneKind.Base : distance <= 2 ? WorldZoneKind.Near : distance <= 4 ? WorldZoneKind.Danger : WorldZoneKind.Far;
			tiles.Add(new MapTile030(x, y, kind, Math.Max(0, distance)));
		}
		return tiles;
	}
}
