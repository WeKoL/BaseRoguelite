public class GameMenuContext
{
	public MenuSection Section { get; set; } = MenuSection.Inventory;
	public BaseActionId FocusedBaseAction { get; set; } = BaseActionId.None;
	public string SourceId { get; set; } = "manual_open";
}
	
