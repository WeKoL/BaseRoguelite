using Xunit;

public sealed class InventoryNotificationTextBuilderTests
{
	[Fact]
	public void BuildPickup_ReturnsPlusText_WithFinalInventoryAmount()
	{
		string text = InventoryNotificationTextBuilder.BuildPickup("Металл", 5, 12);

		Assert.Equal("+5 Металл (12)", text);
	}

	[Fact]
	public void BuildDrop_ReturnsMinusText_WithFinalInventoryAmount()
	{
		string text = InventoryNotificationTextBuilder.BuildDrop("Металл", 5, 7);

		Assert.Equal("-5 Металл (7)", text);
	}

	[Fact]
	public void BuildPickup_ReturnsEmpty_WhenDisplayNameIsMissing()
	{
		string text = InventoryNotificationTextBuilder.BuildPickup("", 5, 12);

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public void BuildDrop_ReturnsEmpty_WhenAmountIsNotPositive()
	{
		string text = InventoryNotificationTextBuilder.BuildDrop("Металл", 0, 7);

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public void BuildDrop_ReturnsEmpty_WhenFinalInventoryAmountIsNegative()
	{
		string text = InventoryNotificationTextBuilder.BuildDrop("Металл", 3, -1);

		Assert.Equal(string.Empty, text);
	}
}
