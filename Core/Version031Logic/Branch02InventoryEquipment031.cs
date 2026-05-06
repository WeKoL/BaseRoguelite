using System;
using System.Collections.Generic;
using System.Linq;

public enum InventoryActionKind031
{
	Inspect,
	Use,
	Equip,
	Unequip,
	SplitStack,
	MoveToStorage,
	Drop,
	Repair
}

public sealed class InventoryActionMenuPlan031
{
	public IReadOnlyList<InventoryActionKind031> Actions { get; }
	public int VisualRows => Actions.Count;

	private InventoryActionMenuPlan031(IEnumerable<InventoryActionKind031> actions)
	{
		Actions = actions.ToList();
	}

	public static InventoryActionMenuPlan031 ForInventoryItem(bool isConsumable, bool isEquipment, bool canMoveToStorage)
	{
		List<InventoryActionKind031> actions = new() { InventoryActionKind031.Inspect };
		if (isConsumable) actions.Add(InventoryActionKind031.Use);
		if (isEquipment) actions.Add(InventoryActionKind031.Equip);
		if (canMoveToStorage) actions.Add(InventoryActionKind031.MoveToStorage);
		actions.Add(InventoryActionKind031.Drop);
		return new InventoryActionMenuPlan031(actions);
	}

	public static InventoryActionMenuPlan031 ForEquippedItem(bool damaged)
	{
		List<InventoryActionKind031> actions = new() { InventoryActionKind031.Inspect, InventoryActionKind031.Unequip };
		if (damaged) actions.Add(InventoryActionKind031.Repair);
		return new InventoryActionMenuPlan031(actions);
	}
}

public sealed class GearComparison031
{
	public int ArmorDelta { get; }
	public int DamageDelta { get; }
	public float WeightDelta { get; }
	public string Summary { get; }
	public bool IsUpgrade => ArmorDelta > 0 || DamageDelta > 0 || WeightDelta < 0;

	public GearComparison031(int currentArmor, int candidateArmor, int currentDamage, int candidateDamage, float currentWeight, float candidateWeight)
	{
		ArmorDelta = candidateArmor - currentArmor;
		DamageDelta = candidateDamage - currentDamage;
		WeightDelta = candidateWeight - currentWeight;
		Summary = BuildSummary();
	}

	private string BuildSummary()
	{
		List<string> parts = new();
		if (ArmorDelta != 0) parts.Add($"armor {(ArmorDelta > 0 ? "+" : string.Empty)}{ArmorDelta}");
		if (DamageDelta != 0) parts.Add($"damage {(DamageDelta > 0 ? "+" : string.Empty)}{DamageDelta}");
		if (Math.Abs(WeightDelta) > 0.001f) parts.Add($"weight {(WeightDelta > 0 ? "+" : string.Empty)}{WeightDelta:0.##}");
		return parts.Count == 0 ? "same_stats" : string.Join(", ", parts);
	}
}
