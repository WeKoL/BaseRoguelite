using Xunit;

public sealed class Version30GatheringTests
{
	[Fact]
	public void ResourceNode_RequiresToolWhenConfigured()
	{
		ResourceNodeState node = new("stone", 1, 1, "pickaxe");
		GatherResult result = node.TryGather(new InventoryState(3, 50), new InventoryItemDefinition("stone", 10, 1));
		Assert.False(result.Succeeded);
		Assert.Equal("tool_required", result.Reason);
	}

	[Fact]
	public void GatheringToolLogic_UsesBestAvailableBonus()
	{
		InventoryState tools = new(3, 50);
		tools.TryAddItem(new InventoryItemDefinition("pickaxe", 1, 1), 1);
		int bonus = GatheringToolLogic.GetBonusAmount(tools, new[]
		{
			new ToolEfficiencyDefinition("knife", 1),
			new ToolEfficiencyDefinition("pickaxe", 3)
		});
		Assert.Equal(3, bonus);
	}

	[Fact]
	public void Regeneration_RestoresChargesOverTime()
	{
		ResourceNodeRegenerationState regen = new(3, 0, 10);
		Assert.Equal(1, regen.Tick(10));
		Assert.Equal(1, regen.CurrentCharges);
	}
}
