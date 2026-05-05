public static class CombatAttackLogic
{
	public static CombatAttackResult TryMeleeAttack(PlayerStatsState attackerStats, EquipmentState equipment, EnemyState target, WeaponAttackDefinition weapon)
	{
		if (attackerStats == null || target == null || weapon == null) return CombatAttackResult.Fail("invalid");
		if (attackerStats.IsDead) return CombatAttackResult.Fail("attacker_dead");
		if (target.IsDead) return CombatAttackResult.Fail("target_dead");
		if (!attackerStats.TrySpendStamina(weapon.StaminaCost)) return CombatAttackResult.Fail("not_enough_stamina");
		int damage = target.TakeDamage(weapon.Damage);
		if (equipment != null && weapon.DurabilityCost > 0) equipment.ReduceDurability(EquipmentSlotId.PrimaryWeapon, weapon.DurabilityCost);
		return CombatAttackResult.Success(damage, target.IsDead);
	}
}
