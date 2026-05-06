using System;
using System.Collections.Generic;
using System.Linq;

public sealed class BranchWorkItem031
{
	public int BranchNumber { get; }
	public string Title { get; }
	public int EstimatedHours { get; }
	public bool HasTest { get; }
	public bool HasGameplayHook { get; }

	public BranchWorkItem031(int branchNumber, string title, int estimatedHours, bool hasTest, bool hasGameplayHook)
	{
		BranchNumber = Math.Max(1, branchNumber);
		Title = string.IsNullOrWhiteSpace(title) ? "untitled" : title.Trim();
		EstimatedHours = Math.Max(0, estimatedHours);
		HasTest = hasTest;
		HasGameplayHook = hasGameplayHook;
	}
}

public sealed class DevelopmentBacklog031
{
	private readonly List<BranchWorkItem031> _items = new();
	public IReadOnlyList<BranchWorkItem031> Items => _items;
	public int TotalEstimatedHours => _items.Sum(x => x.EstimatedHours);
	public int CoveredBranches => _items.Select(x => x.BranchNumber).Distinct().Count();
	public bool CoversEveryBranch => CoveredBranches >= 14;

	public void Add(BranchWorkItem031 item)
	{
		if (item != null)
			_items.Add(item);
	}

	public IReadOnlyList<BranchWorkItem031> ForBranch(int branchNumber)
	{
		return _items.Where(x => x.BranchNumber == branchNumber).ToList();
	}

	public IReadOnlyList<BranchWorkItem031> MissingTests()
	{
		return _items.Where(x => !x.HasTest).ToList();
	}
}

public sealed class ReleaseChecklist031
{
	public bool GodotBuildPassed { get; private set; }
	public bool UnitTestsPassed { get; private set; }
	public bool NoGeneratedFoldersTracked { get; private set; }
	public bool VersionNotesUpdated { get; private set; }
	public bool SaveMigrationReady { get; private set; }

	public void MarkGodotBuildPassed() => GodotBuildPassed = true;
	public void MarkUnitTestsPassed() => UnitTestsPassed = true;
	public void MarkNoGeneratedFoldersTracked() => NoGeneratedFoldersTracked = true;
	public void MarkVersionNotesUpdated() => VersionNotesUpdated = true;
	public void MarkSaveMigrationReady() => SaveMigrationReady = true;

	public bool CanTagRelease => GodotBuildPassed && UnitTestsPassed && NoGeneratedFoldersTracked && VersionNotesUpdated && SaveMigrationReady;

	public IReadOnlyList<string> GetMissingItems()
	{
		List<string> missing = new();
		if (!GodotBuildPassed) missing.Add("godot_build");
		if (!UnitTestsPassed) missing.Add("unit_tests");
		if (!NoGeneratedFoldersTracked) missing.Add("gitignore_generated_folders");
		if (!VersionNotesUpdated) missing.Add("version_notes");
		if (!SaveMigrationReady) missing.Add("save_migration");
		return missing;
	}
}
