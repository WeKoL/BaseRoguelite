public sealed class SurvivalCondition030
{
	public string Severity { get; }
	public string Advice { get; }
	public bool ShouldReturnToBase { get; }
	public SurvivalCondition030(string severity, string advice, bool shouldReturnToBase)
	{
		Severity = severity ?? "normal";
		Advice = advice ?? string.Empty;
		ShouldReturnToBase = shouldReturnToBase;
	}
}

public static class SurvivalConditionPlanner030
{
	public static SurvivalCondition030 Evaluate(PlayerStatsState stats, SurvivalNeedsState needs, int dangerLevel)
	{
		if (stats == null || needs == null) return new SurvivalCondition030("unknown", "Нет данных игрока", true);
		if (stats.CurrentHealth <= stats.MaxHealth * 0.25f || needs.IsCritical) return new SurvivalCondition030("critical", "Срочно возвращайся на базу", true);
		if (dangerLevel >= 4 && (stats.CurrentHealth <= stats.MaxHealth * 0.55f || needs.IsHungry || needs.IsThirsty)) return new SurvivalCondition030("danger", "Риск высокий: лучше закончить вылазку", true);
		if (needs.IsHungry || needs.IsThirsty) return new SurvivalCondition030("warning", "Используй еду или воду", false);
		return new SurvivalCondition030("normal", "Можно продолжать экспедицию", false);
	}
}
