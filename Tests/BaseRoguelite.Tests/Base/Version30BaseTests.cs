using Xunit;

public sealed class Version30BaseTests
{
	[Fact]
	public void FacilityEffects_ScaleWithLevels()
	{
		Assert.True(BaseFacilityEffectCalculator.GetStorageEntryLimit(2) > BaseFacilityEffectCalculator.GetStorageEntryLimit(0));
		Assert.True(BaseFacilityEffectCalculator.GetHealingAmount(2) > BaseFacilityEffectCalculator.GetHealingAmount(0));
	}

	[Fact]
	public void Energy_ReserveAndRelease_Works()
	{
		BaseEnergyState energy = new(10);
		Assert.True(energy.TryReserve(6));
		Assert.Equal(4, energy.Free);
		energy.Release(3);
		Assert.Equal(7, energy.Free);
	}

	[Fact]
	public void Defense_BreachesWhenThreatIsHigherThanProtection()
	{
		BaseDefenseState defense = new(5, 2);
		defense.ApplyThreat(8);
		Assert.True(defense.IsBreached);
	}
}
