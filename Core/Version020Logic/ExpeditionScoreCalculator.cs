using System;

public sealed class ExpeditionScore
{
	public int ResourceScore { get; }
	public int CombatScore { get; }
	public int SurvivalPenalty { get; }
	public int TotalScore => Math.Max(0, ResourceScore + CombatScore - SurvivalPenalty);
	public string Grade { get; }

	public ExpeditionScore(int resourceScore, int combatScore, int survivalPenalty, string grade)
	{
		ResourceScore = Math.Max(0, resourceScore);
		CombatScore = Math.Max(0, combatScore);
		SurvivalPenalty = Math.Max(0, survivalPenalty);
		Grade = string.IsNullOrWhiteSpace(grade) ? "D" : grade;
	}
}

public static class ExpeditionScoreCalculator
{
	public static ExpeditionScore Calculate(int resourcesCollected, int enemiesDefeated, int damageTaken)
	{
		int resource = Math.Max(0, resourcesCollected) * 3;
		int combat = Math.Max(0, enemiesDefeated) * 10;
		int penalty = Math.Max(0, damageTaken) / 2;
		int total = Math.Max(0, resource + combat - penalty);
		string grade = total >= 100 ? "S" : total >= 60 ? "A" : total >= 30 ? "B" : total >= 10 ? "C" : "D";
		return new ExpeditionScore(resource, combat, penalty, grade);
	}
}
