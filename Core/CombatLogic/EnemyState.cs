public sealed class EnemyState
{
	public string Id { get; }
	public int MaxHealth { get; }
	public int CurrentHealth { get; private set; }
	public int Armor { get; }
	public int TouchDamage { get; }
	public bool IsDead => CurrentHealth <= 0;
	public EnemyState(string id, int maxHealth, int armor, int touchDamage) { Id = id ?? string.Empty; MaxHealth = maxHealth <= 0 ? 1 : maxHealth; CurrentHealth = MaxHealth; Armor = armor < 0 ? 0 : armor; TouchDamage = touchDamage < 0 ? 0 : touchDamage; }
	public int TakeDamage(int rawDamage) { if (rawDamage <= 0 || IsDead) return 0; int finalDamage = CombatDamageCalculator.CalculateFinalDamage(rawDamage, Armor); int before = CurrentHealth; CurrentHealth = System.Math.Max(0, CurrentHealth - finalDamage); return before - CurrentHealth; }
}
