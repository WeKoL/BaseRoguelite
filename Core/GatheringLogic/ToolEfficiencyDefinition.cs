public sealed class ToolEfficiencyDefinition
{
	public string ToolItemId { get; }
	public int BonusAmount { get; }
	public int ChargeCostReduction { get; }
	public ToolEfficiencyDefinition(string toolItemId, int bonusAmount, int chargeCostReduction = 0) { ToolItemId = toolItemId ?? string.Empty; BonusAmount = bonusAmount < 0 ? 0 : bonusAmount; ChargeCostReduction = chargeCostReduction < 0 ? 0 : chargeCostReduction; }
}
