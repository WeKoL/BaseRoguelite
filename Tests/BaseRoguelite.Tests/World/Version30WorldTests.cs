using Xunit;

public sealed class Version30WorldTests
{
	[Fact]
	public void ExpeditionTimer_ExpiresAtMaxSeconds()
	{
		ExpeditionTimerState timer = new(10);
		timer.Tick(15);
		Assert.True(timer.IsExpired);
		Assert.Equal(10, timer.ElapsedSeconds);
	}

	[Fact]
	public void BaseDirectionHint_ReturnsReadableDirection()
	{
		BaseDirectionHint hint = BaseDirectionHintBuilder.Build(0, 0, 10, -10);
		Assert.Equal("север-восток", hint.Text);
		Assert.True(hint.Distance > 0);
	}

	[Fact]
	public void Extraction_MovesInventoryItemsToStorage()
	{
		InventoryState inventory = new(2, 50);
		inventory.TryAddItem(new InventoryItemDefinition("wood", 10, 1), 4);
		StorageState storage = new();
		ExtractionResult result = ExpeditionExtractionLogic.TryExtractToBase(inventory, storage);
		Assert.True(result.Succeeded);
		Assert.Equal(4, result.ItemsSaved);
		Assert.Equal(4, storage.GetTotalAmount("wood"));
	}
}
