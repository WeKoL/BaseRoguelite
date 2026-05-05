public sealed class ExtractionResult
{
	public bool Succeeded { get; }
	public string Reason { get; }
	public int ItemsSaved { get; }
	private ExtractionResult(bool succeeded, string reason, int itemsSaved) { Succeeded = succeeded; Reason = reason ?? string.Empty; ItemsSaved = itemsSaved < 0 ? 0 : itemsSaved; }
	public static ExtractionResult Success(int itemsSaved) => new(true, string.Empty, itemsSaved);
	public static ExtractionResult Fail(string reason) => new(false, reason, 0);
}
