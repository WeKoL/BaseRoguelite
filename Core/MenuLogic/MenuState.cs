public class MenuState
{
	public bool IsOpen { get; private set; }
	public MenuSection CurrentRootSection { get; private set; } = MenuSection.Inventory;
	public BaseActionId CurrentSubAction { get; private set; } = BaseActionId.None;

	public bool IsSubViewOpen => IsOpen && CurrentSubAction != BaseActionId.None;

	public void OpenDefault()
	{
		IsOpen = true;
		CurrentRootSection = MenuSection.Inventory;
		CurrentSubAction = BaseActionId.None;
	}

	public void OpenWithContext(MenuSection section, BaseActionId focusedBaseAction)
	{
		IsOpen = true;
		CurrentRootSection = section;

		if (section == MenuSection.Base && focusedBaseAction != BaseActionId.None)
			CurrentSubAction = focusedBaseAction;
		else
			CurrentSubAction = BaseActionId.None;
	}

	public void SelectRootSection(MenuSection section)
	{
		CurrentRootSection = section;
		CurrentSubAction = BaseActionId.None;
	}

	public bool TryOpenBaseAction(BaseActionId actionId, bool canUseBaseActions)
	{
		if (!IsOpen || !canUseBaseActions || actionId == BaseActionId.None)
			return false;

		CurrentRootSection = MenuSection.Base;
		CurrentSubAction = actionId;
		return true;
	}

	public void CloseSubView()
	{
		CurrentSubAction = BaseActionId.None;
	}

	public void CloseMenu()
	{
		IsOpen = false;
		CurrentSubAction = BaseActionId.None;
	}

	public MenuBackActionResult HandleBackAction()
	{
		if (!IsOpen)
			return MenuBackActionResult.None;

		if (IsSubViewOpen)
		{
			CloseSubView();
			return MenuBackActionResult.ClosedSubView;
		}

		CloseMenu();
		return MenuBackActionResult.ClosedMenu;
	}
}
