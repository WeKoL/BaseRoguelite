public sealed class GameplayBalanceWarning
{
	public string Code { get; }
	public string Message { get; }
	public GameplayBalanceWarning(string code, string message) { Code = code ?? string.Empty; Message = message ?? string.Empty; }
}
