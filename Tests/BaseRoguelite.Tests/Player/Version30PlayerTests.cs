using Xunit;

public sealed class Version30PlayerTests
{
	[Fact]
	public void SurvivalNeeds_ConsumeTriggersHungryAndThirsty()
	{
		SurvivalNeedsState needs = new(100, 100);
		needs.Consume(80, 80);
		Assert.True(needs.IsHungry);
		Assert.True(needs.IsThirsty);
		Assert.Equal(10, needs.Eat(10));
		Assert.Equal(10, needs.Drink(10));
	}

	[Fact]
	public void StatusEffectCollection_TicksDamageAndRemovesExpired()
	{
		PlayerStatsState stats = new(50);
		PlayerStatusEffectCollection effects = new();
		effects.Add(new PlayerStatusEffectState(PlayerStatusEffectKind.Bleeding, 2, 3));
		int damage = effects.TickAll(stats, 2);
		Assert.Equal(6, damage);
		Assert.False(effects.Has(PlayerStatusEffectKind.Bleeding));
	}

	[Fact]
	public void Stamina_SpendAndRestore_Works()
	{
		PlayerStatsState stats = new(100, 0, 20);
		Assert.True(stats.TrySpendStamina(12));
		Assert.Equal(8, stats.CurrentStamina);
		Assert.Equal(5, stats.RestoreStamina(5));
	}
}
