using System;

public sealed class PlayerStatsState
{
	public int MaxHealth { get; private set; }
	public int CurrentHealth { get; private set; }
	public int Armor { get; private set; }
	public int MaxStamina { get; private set; }
	public int CurrentStamina { get; private set; }
	public bool IsDead => CurrentHealth <= 0;

	public PlayerStatsState(int maxHealth, int armor = 0) : this(maxHealth, armor, 100) { }
	public PlayerStatsState(int maxHealth, int armor, int maxStamina)
	{
		MaxHealth = Math.Max(1, maxHealth); CurrentHealth = MaxHealth; Armor = Math.Max(0, armor); MaxStamina = Math.Max(1, maxStamina); CurrentStamina = MaxStamina;
	}
	public int Heal(int amount) { if (amount <= 0 || IsDead) return 0; int before = CurrentHealth; CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount); return CurrentHealth - before; }
	public int TakeDamage(int rawDamage) { if (rawDamage <= 0 || IsDead) return 0; int final = CombatDamageCalculator.CalculateFinalDamage(rawDamage, Armor); int before = CurrentHealth; CurrentHealth = Math.Max(0, CurrentHealth - final); return before - CurrentHealth; }
	public int TakeDirectDamage(int damage) { if (damage <= 0 || IsDead) return 0; int before = CurrentHealth; CurrentHealth = Math.Max(0, CurrentHealth - damage); return before - CurrentHealth; }
	public void SetArmor(int armor) { Armor = Math.Max(0, armor); }
	public bool TrySpendStamina(int amount) { if (amount <= 0) return true; if (CurrentStamina < amount) return false; CurrentStamina -= amount; return true; }
	public int RestoreStamina(int amount) { if (amount <= 0) return 0; int before = CurrentStamina; CurrentStamina = Math.Min(MaxStamina, CurrentStamina + amount); return CurrentStamina - before; }
	public float GetStaminaRatio() { return MaxStamina <= 0 ? 0f : Math.Clamp(CurrentStamina / (float)MaxStamina, 0f, 1f); }
}
