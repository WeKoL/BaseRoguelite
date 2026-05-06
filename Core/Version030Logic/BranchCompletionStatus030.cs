using System;

public enum BranchCompletionStatus030
{
	NotStarted = 0,
	CoreReady = 1,
	PlayablePrototype = 2,
	Integrated = 3,
	Polished = 4
}

public sealed class BranchCompletionItem030
{
	public int BranchNumber { get; }
	public string BranchName { get; }
	public BranchCompletionStatus030 Status { get; }
	public int TestCount { get; }
	public bool HasVisibleGameplayHook { get; }
	public string Notes { get; }

	public BranchCompletionItem030(int branchNumber, string branchName, BranchCompletionStatus030 status, int testCount, bool hasVisibleGameplayHook, string notes = "")
	{
		BranchNumber = Math.Max(1, branchNumber);
		BranchName = string.IsNullOrWhiteSpace(branchName) ? $"Branch {BranchNumber}" : branchName;
		Status = status;
		TestCount = Math.Max(0, testCount);
		HasVisibleGameplayHook = hasVisibleGameplayHook;
		Notes = notes ?? string.Empty;
	}
}
