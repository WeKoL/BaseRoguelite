using System;
using System.Collections.Generic;
using System.Linq;

public sealed class QualityGate040
{
	public QualityResult040 Evaluate(CoreManifest040 manifest, SaveValidation040 saveValidation, int factTestsCount, bool hasVersionNotes)
	{
		List<string> blockers = new();
		if (manifest == null || !manifest.HasAllBranches) blockers.Add("not_all_branches_covered");
		if (manifest != null && !manifest.HasMinimumTestsPerBranch(1)) blockers.Add("branch_without_test");
		if (saveValidation == null || !saveValidation.IsValid) blockers.Add("save_validation_failed");
		if (factTestsCount < 14) blockers.Add("too_few_tests");
		if (!hasVersionNotes) blockers.Add("version_notes_missing");
		return new QualityResult040(blockers);
	}
}

public sealed class QualityResult040
{
	public IReadOnlyList<string> Blockers { get; }
	public bool Passed => Blockers.Count == 0;
	public QualityResult040(IReadOnlyList<string> blockers) { Blockers = blockers ?? Array.Empty<string>(); }
}

public sealed class BalanceRules040
{
	public int MaxSafeCarryWeight { get; init; } = 80;
	public int MaxSingleEnemyDamage { get; init; } = 20;
	public int MinimumWaterWarning { get; init; } = 20;
	public int MinimumFoodWarning { get; init; } = 20;

	public IReadOnlyList<string> Validate(ItemRegistry040 items)
	{
		List<string> warnings = new();
		foreach (ItemDefinition040 item in items.Items.Values)
		{
			if (item.Weight > MaxSafeCarryWeight) warnings.Add($"too_heavy:{item.Id}");
			if (item.Damage > MaxSingleEnemyDamage && item.Category != ItemCategory040.Weapon) warnings.Add($"unexpected_damage:{item.Id}");
		}
		return warnings;
	}
}
