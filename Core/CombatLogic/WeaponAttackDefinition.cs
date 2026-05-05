public sealed class WeaponAttackDefinition
{
	public string WeaponItemId { get; }
	public int Damage { get; }
	public int StaminaCost { get; }
	public float Range { get; }
	public int DurabilityCost { get; }
	public WeaponAttackDefinition(string weaponItemId, int damage, int staminaCost, float range, int durabilityCost = 1) { WeaponItemId = weaponItemId ?? string.Empty; Damage = damage <= 0 ? 1 : damage; StaminaCost = staminaCost < 0 ? 0 : staminaCost; Range = range <= 0f ? 32f : range; DurabilityCost = durabilityCost < 0 ? 0 : durabilityCost; }
}
