public static class DamageFeedbackPlanner
{
	public static DamageFeedbackPlan Build(int damage, int criticalThreshold) { damage = System.Math.Max(0, damage); float shake = damage <= 0 ? 0f : System.MathF.Min(1.5f, 0.2f + damage / 50f); return new DamageFeedbackPlan(damage, shake, damage >= criticalThreshold && criticalThreshold > 0); }
}
