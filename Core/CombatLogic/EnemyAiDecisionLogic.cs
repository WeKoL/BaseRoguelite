public static class EnemyAiDecisionLogic
{
	public static EnemyAiDecision Decide(float distanceToPlayer, float aggroRange, float attackRange, bool lowHealth)
	{
		if (lowHealth && distanceToPlayer <= attackRange) return EnemyAiDecision.Retreat; if (distanceToPlayer <= attackRange) return EnemyAiDecision.Attack; if (distanceToPlayer <= aggroRange) return EnemyAiDecision.Chase; return EnemyAiDecision.Idle;
	}
}
