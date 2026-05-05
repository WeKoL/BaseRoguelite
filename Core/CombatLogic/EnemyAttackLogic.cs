public static class EnemyAttackLogic
{
	public static CombatAttackResult TryTouchAttack(EnemyState enemy, PlayerStatsState player, AttackCooldownState cooldown)
	{
		if (enemy == null || player == null || cooldown == null) return CombatAttackResult.Fail("invalid"); if (enemy.IsDead) return CombatAttackResult.Fail("enemy_dead"); if (player.IsDead) return CombatAttackResult.Fail("player_dead"); if (!cooldown.TryUse()) return CombatAttackResult.Fail("cooldown"); int damage = player.TakeDamage(enemy.TouchDamage); return CombatAttackResult.Success(damage, player.IsDead);
	}
}
