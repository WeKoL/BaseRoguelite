using System.Collections.Generic;
using System.Linq;

public sealed class Version020CoverageReport
{
	public IReadOnlyList<string> CompletedBranches { get; }
	public IReadOnlyList<string> DeferredBranches { get; }
	public bool HasDeferredWork => DeferredBranches.Count > 0;

	public Version020CoverageReport(IEnumerable<string> completed, IEnumerable<string> deferredWork)
	{
		CompletedBranches = (completed ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		DeferredBranches = (deferredWork ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
	}
}
