using Xunit;

public sealed class Version30TechnicalTests
{
	[Fact]
	public void ReadinessReport_UsesWeightsAndMissingCount()
	{
		FeatureReadinessReport report = FeatureReadinessChecker.BuildReport(new[]
		{
			FeatureReadinessChecker.Require("items", "Items", true, 2),
			FeatureReadinessChecker.Require("save", "Save", false, 3)
		});
		Assert.Equal(5, report.TotalWeight);
		Assert.Equal(2, report.DoneWeight);
		Assert.Equal(1, report.MissingCount);
		Assert.False(report.IsReady);
	}

	[Fact]
	public void BalanceValidator_WarnsAboutTinyInventoryAndStorage()
	{
		var warnings = GameplayBalanceValidator.ValidateInventoryAndStorage(new InventoryState(2, 3), new StorageState(2));
		Assert.Contains(warnings, w => w.Code == "inventory_too_small");
		Assert.Contains(warnings, w => w.Code == "storage_too_small");
	}
}
