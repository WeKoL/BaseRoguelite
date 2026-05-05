using Xunit;

public sealed class Version30UiTests
{
	[Fact]
	public void ConfirmationPrompt_RequiresRareDropConfirmation()
	{
		Assert.True(ConfirmationPromptLogic.RequiresConfirmation(DangerousActionKind.DropItem, ItemRarity.Rare, 1));
		Assert.True(ConfirmationPromptLogic.RequiresConfirmation(DangerousActionKind.DropItem, ItemRarity.Common, 10));
		Assert.False(ConfirmationPromptLogic.RequiresConfirmation(DangerousActionKind.DropItem, ItemRarity.Common, 1));
	}

	[Fact]
	public void Tooltip_BuildsWeightAndDurabilityLines()
	{
		Assert.Equal("Вес: 3 кг", ItemTooltipDetailsBuilder.BuildWeightLine(3, 1));
		Assert.Equal("Прочность: 5/10", ItemTooltipDetailsBuilder.BuildDurabilityLine(5, 10));
	}

	[Fact]
	public void InventoryHint_ShowsWeight()
	{
		InventoryState inventory = new(2, 10);
		inventory.TryAddItem(new InventoryItemDefinition("wood", 10, 1), 3);
		Assert.Contains("3", InventoryHintTextBuilder.BuildWeightHint(inventory));
	}
}
