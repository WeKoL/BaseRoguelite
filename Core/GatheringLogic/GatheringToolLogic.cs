using System.Collections.Generic;
public static class GatheringToolLogic
{
	public static int GetBonusAmount(InventoryState toolInventory, IEnumerable<ToolEfficiencyDefinition> definitions)
	{
		if (toolInventory == null || definitions == null) return 0; int best = 0; foreach (ToolEfficiencyDefinition d in definitions) if (d != null && toolInventory.GetTotalAmount(d.ToolItemId) > 0 && d.BonusAmount > best) best = d.BonusAmount; return best;
	}
}
