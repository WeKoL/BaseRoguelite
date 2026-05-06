#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class FacilityDependencyGraph031
{
	private readonly Dictionary<string, List<string>> _dependencies = new();
	public void AddDependency(string facilityId, string requiredFacilityId)
	{
		if (string.IsNullOrWhiteSpace(facilityId) || string.IsNullOrWhiteSpace(requiredFacilityId)) return;
		if (!_dependencies.ContainsKey(facilityId)) _dependencies[facilityId] = new List<string>();
		if (!_dependencies[facilityId].Contains(requiredFacilityId)) _dependencies[facilityId].Add(requiredFacilityId);
	}
	public bool CanBuild(string facilityId, IEnumerable<string> builtFacilities)
	{
		HashSet<string> built = new(builtFacilities ?? Array.Empty<string>());
		return !_dependencies.TryGetValue(facilityId ?? string.Empty, out var deps) || deps.All(built.Contains);
	}
}

public sealed class BaseProjectQueue031
{
	private readonly Queue<string> _projects = new();
	public int Count => _projects.Count;
	public void Enqueue(string projectId)
	{
		if (!string.IsNullOrWhiteSpace(projectId)) _projects.Enqueue(projectId);
	}
	public string? CompleteNext() => _projects.Count == 0 ? null : _projects.Dequeue();
}

public sealed class BaseComfortCalculator031
{
	public int Calculate(int medLevel, int generatorLevel, int wallLevel, int storedFoodStacks)
	{
		return Math.Max(0, medLevel) * 8 + Math.Max(0, generatorLevel) * 6 + Math.Max(0, wallLevel) * 5 + Math.Min(20, Math.Max(0, storedFoodStacks) * 2);
	}
}
