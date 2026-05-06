using System.Collections.Generic;
using System.Linq;

public sealed class JournalChapterState030
{
	private readonly HashSet<string> _completed = new();
	public string ChapterId { get; }
	public IReadOnlyCollection<string> CompletedSteps => _completed;
	public JournalChapterState030(string chapterId) { ChapterId = chapterId ?? string.Empty; }
	public bool CompleteStep(string stepId) => !string.IsNullOrWhiteSpace(stepId) && _completed.Add(stepId);
	public bool IsComplete(IEnumerable<string> requiredSteps)
	{
		return (requiredSteps ?? System.Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).All(x => _completed.Contains(x));
	}
}
