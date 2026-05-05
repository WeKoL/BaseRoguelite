using Xunit;

public sealed class Version30CombatTests
{
	[Fact]
	public void MeleeAttack_SpendsStaminaAndDamagesEnemy()
	{
		PlayerStatsState player = new(100, 0, 20);
		EnemyState enemy = new("rat", 20, 0, 2);
		CombatAttackResult result = CombatAttackLogic.TryMeleeAttack(player, null, enemy, new WeaponAttackDefinition("club", 6, 5, 30));
		Assert.True(result.Succeeded);
		Assert.Equal(14, enemy.CurrentHealth);
		Assert.Equal(15, player.CurrentStamina);
	}

	[Fact]
	public void EnemyAiDecision_ChoosesAttackInsideRange()
	{
		Assert.Equal(EnemyAiDecision.Attack, EnemyAiDecisionLogic.Decide(10, 100, 20, false));
		Assert.Equal(EnemyAiDecision.Retreat, EnemyAiDecisionLogic.Decide(10, 100, 20, true));
	}

	[Fact]
	public void EnemyAttack_UsesCooldown()
	{
		EnemyState enemy = new("rat", 10, 0, 5);
		PlayerStatsState player = new(20);
		AttackCooldownState cooldown = new(1000);
		Assert.True(EnemyAttackLogic.TryTouchAttack(enemy, player, cooldown).Succeeded);
		Assert.False(EnemyAttackLogic.TryTouchAttack(enemy, player, cooldown).Succeeded);
		cooldown.Tick(1000);
		Assert.True(EnemyAttackLogic.TryTouchAttack(enemy, player, cooldown).Succeeded);
	}
}
