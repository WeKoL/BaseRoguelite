using System.Collections.Generic;
public sealed class SaveValidationResult
{
	private readonly List<string> _errors = new();
	public IReadOnlyList<string> Errors => _errors;
	public bool IsValid => _errors.Count == 0;
	public void AddError(string error) { if (!string.IsNullOrWhiteSpace(error)) _errors.Add(error); }
}
