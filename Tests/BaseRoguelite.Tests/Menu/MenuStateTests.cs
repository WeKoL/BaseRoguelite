using Xunit;

namespace BaseRoguelite.Tests.Menu;

public class MenuStateTests
{
	[Fact]
	public void NewState_IsClosed_ByDefault()
	{
		var state = new MenuState();

		Assert.False(state.IsOpen);
		Assert.Equal(MenuSection.Inventory, state.CurrentRootSection);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
		Assert.False(state.IsSubViewOpen);
	}

	[Fact]
	public void OpenDefault_OpensInventoryRoot()
	{
		var state = new MenuState();

		state.OpenDefault();

		Assert.True(state.IsOpen);
		Assert.Equal(MenuSection.Inventory, state.CurrentRootSection);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
		Assert.False(state.IsSubViewOpen);
	}

	[Fact]
	public void OpenWithContext_BaseAndStorage_OpensSubView()
	{
		var state = new MenuState();

		state.OpenWithContext(MenuSection.Base, BaseActionId.Storage);

		Assert.True(state.IsOpen);
		Assert.Equal(MenuSection.Base, state.CurrentRootSection);
		Assert.Equal(BaseActionId.Storage, state.CurrentSubAction);
		Assert.True(state.IsSubViewOpen);
	}

	[Fact]
	public void OpenWithContext_Inventory_IgnoresSubAction()
	{
		var state = new MenuState();

		state.OpenWithContext(MenuSection.Inventory, BaseActionId.Storage);

		Assert.True(state.IsOpen);
		Assert.Equal(MenuSection.Inventory, state.CurrentRootSection);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
		Assert.False(state.IsSubViewOpen);
	}

	[Fact]
	public void SelectRootSection_ClosesSubView()
	{
		var state = new MenuState();
		state.OpenWithContext(MenuSection.Base, BaseActionId.Storage);

		state.SelectRootSection(MenuSection.Journal);

		Assert.Equal(MenuSection.Journal, state.CurrentRootSection);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
		Assert.False(state.IsSubViewOpen);
	}

	[Fact]
	public void TryOpenBaseAction_Fails_WhenActionUnavailable()
	{
		var state = new MenuState();
		state.OpenDefault();

		bool opened = state.TryOpenBaseAction(BaseActionId.Storage, canUseBaseActions: false);

		Assert.False(opened);
		Assert.Equal(MenuSection.Inventory, state.CurrentRootSection);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
	}

	[Fact]
	public void TryOpenBaseAction_OpensSubView_WhenAllowed()
	{
		var state = new MenuState();
		state.OpenDefault();

		bool opened = state.TryOpenBaseAction(BaseActionId.Heal, canUseBaseActions: true);

		Assert.True(opened);
		Assert.Equal(MenuSection.Base, state.CurrentRootSection);
		Assert.Equal(BaseActionId.Heal, state.CurrentSubAction);
		Assert.True(state.IsSubViewOpen);
	}

	[Fact]
	public void HandleBackAction_ClosesSubView_First()
	{
		var state = new MenuState();
		state.OpenWithContext(MenuSection.Base, BaseActionId.Craft);

		var result = state.HandleBackAction();

		Assert.Equal(MenuBackActionResult.ClosedSubView, result);
		Assert.True(state.IsOpen);
		Assert.False(state.IsSubViewOpen);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
	}

	[Fact]
	public void HandleBackAction_ClosesMenu_FromRoot()
	{
		var state = new MenuState();
		state.OpenDefault();

		var result = state.HandleBackAction();

		Assert.Equal(MenuBackActionResult.ClosedMenu, result);
		Assert.False(state.IsOpen);
		Assert.False(state.IsSubViewOpen);
	}

	[Fact]
	public void CloseMenu_AlwaysClosesAndClearsSubView()
	{
		var state = new MenuState();
		state.OpenWithContext(MenuSection.Base, BaseActionId.Storage);

		state.CloseMenu();

		Assert.False(state.IsOpen);
		Assert.Equal(BaseActionId.None, state.CurrentSubAction);
		Assert.False(state.IsSubViewOpen);
	}
}
