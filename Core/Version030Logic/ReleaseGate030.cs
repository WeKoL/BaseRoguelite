using System.Collections.Generic;

public sealed class ReleaseGateResult030
{
	private readonly List<string> _blockers = new();
	public IReadOnlyList<string> Blockers => _blockers;
	public bool CanRelease => _blockers.Count == 0;
	public void AddBlocker(string blocker)
	{
		if (!string.IsNullOrWhiteSpace(blocker)) _blockers.Add(blocker);
	}
}

public static class ReleaseGate030
{
	public static ReleaseGateResult030 Evaluate(BranchCompletionReport030 report, SaveValidationResult saveValidation, int factTestsCount)
	{
		ReleaseGateResult030 result = new();
		if (report == null) result.AddBlocker("branch_report_missing");
		else
		{
			if (!report.CoversAllFourteenBranches) result.AddBlocker("not_all_branches_covered");
			if (!report.HasPlayableHooksForMostBranches) result.AddBlocker("not_enough_visible_hooks");
		}
		if (saveValidation == null || !saveValidation.IsValid) result.AddBlocker("save_validation_failed");
		if (factTestsCount < 140) result.AddBlocker("not_enough_tests_for_030");
		return result;
	}
}
