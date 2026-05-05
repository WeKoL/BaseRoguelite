using Xunit;

public sealed class Version30CraftingTests
{
	[Fact]
	public void Crafting_RespectsRequiredStationLevel()
	{
		StorageState storage = new();
		storage.AddItem("wood", 2, 99);
		CraftRecipe recipe = new("bench_item", "Bench Item", new[] { new CraftIngredient("wood", 2) }, "plank", 1, "workbench", 2);
		BaseProgressState progress = new();
		Assert.False(CraftingLogic.CanCraft(storage, recipe, progress));
		progress.IncreaseUpgradeLevel("workbench");
		progress.IncreaseUpgradeLevel("workbench");
		Assert.True(CraftingLogic.CanCraft(storage, recipe, progress));
	}

	[Fact]
	public void CraftQueue_Tick_CompletesCurrentItem()
	{
		CraftQueueState queue = new(2);
		CraftRecipe recipe = new("r", "R", new[] { new CraftIngredient("wood", 1) }, "plank", 1, string.Empty, 0, 5);
		Assert.True(queue.TryEnqueue(recipe));
		Assert.Null(queue.Tick(3));
		Assert.NotNull(queue.Tick(2));
	}

	[Fact]
	public void StartCraft_RemovesIngredientsAndCancelCanReturnThem()
	{
		StorageState storage = new();
		storage.AddItem("wood", 3, 99);
		CraftRecipe recipe = new("r", "R", new[] { new CraftIngredient("wood", 2) }, "plank", 1, string.Empty, 0, 10);
		CraftQueueState queue = new(1);
		Assert.True(CraftQueueStartLogic.TryStartCraft(storage, queue, recipe));
		Assert.Equal(1, storage.GetTotalAmount("wood"));
		CraftQueueStartLogic.ReturnIngredients(storage, queue.CancelCurrent());
		Assert.Equal(3, storage.GetTotalAmount("wood"));
	}
}
