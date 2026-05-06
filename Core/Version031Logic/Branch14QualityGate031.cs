using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TestCoveragePlan031
{
	private readonly Dictionary<int, int> _testsByBranch = new();
	public void AddTests(int branchNumber, int testCount)
	{
		int key = Math.Max(1, branchNumber);
		_testsByBranch[key] = GetTests(key) + Math.Max(0, testCount);
	}
	public int GetTests(int branchNumber) => _testsByBranch.TryGetValue(branchNumber, out int count) ? count : 0;
	public int BranchesWithTests => _testsByBranch.Count(x => x.Value > 0);
	public int TotalTests => _testsByBranch.Values.Sum();
	public bool CoversAllBranches(int requiredTestsPerBranch = 1) => Enumerable.Range(1, 14).All(x => GetTests(x) >= requiredTestsPerBranch);
}

public sealed class QualityGateResult031
{
	public bool Passed { get; }
	public IReadOnlyList<string> Blockers { get; }
	public QualityGateResult031(bool passed, IEnumerable<string> blockers)
	{
		Passed = passed;
		Blockers = (blockers ?? Array.Empty<string>()).ToList();
	}
}

public static class QualityGate031
{
	public static QualityGateResult031 Evaluate(DevelopmentBacklog031 backlog, TestCoveragePlan031 tests, ReleaseChecklist031 checklist)
	{
		List<string> blockers = new();
		if (backlog == null || !backlog.CoversEveryBranch) blockers.Add("not_all_branches_planned");
		if (tests == null || !tests.CoversAllBranches()) blockers.Add("not_all_branches_tested");
		if (checklist == null || !checklist.CanTagRelease) blockers.Add("release_checklist_incomplete");
		return new QualityGateResult031(blockers.Count == 0, blockers);
	}
}
