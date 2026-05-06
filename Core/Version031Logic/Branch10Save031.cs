using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SaveProfile031
{
	public string SlotName { get; }
	public string Version { get; }
	public DateTime CreatedAtUtc { get; }
	public DateTime UpdatedAtUtc { get; private set; }
	public int PlayMinutes { get; private set; }

	public SaveProfile031(string slotName, string version)
	{
		SlotName = string.IsNullOrWhiteSpace(slotName) ? "slot" : slotName.Trim();
		Version = string.IsNullOrWhiteSpace(version) ? "unknown" : version.Trim();
		CreatedAtUtc = DateTime.UtcNow;
		UpdatedAtUtc = CreatedAtUtc;
	}

	public void AddPlayTime(int minutes)
	{
		PlayMinutes += Math.Max(0, minutes);
		UpdatedAtUtc = DateTime.UtcNow;
	}
}

public sealed class AutosavePolicy031
{
	public int IntervalSeconds { get; }
	public bool SaveOnEnterBase { get; }
	public bool SaveBeforeDangerZone { get; }
	public AutosavePolicy031(int intervalSeconds, bool saveOnEnterBase = true, bool saveBeforeDangerZone = true)
	{
		IntervalSeconds = Math.Max(15, intervalSeconds);
		SaveOnEnterBase = saveOnEnterBase;
		SaveBeforeDangerZone = saveBeforeDangerZone;
	}
	public bool ShouldAutosave(float secondsSinceLastSave, bool enteredBase, bool enteringDangerZone)
	{
		return secondsSinceLastSave >= IntervalSeconds || (SaveOnEnterBase && enteredBase) || (SaveBeforeDangerZone && enteringDangerZone);
	}
}

public sealed class SaveIntegrityReport031
{
	public IReadOnlyList<string> Problems { get; }
	public bool IsValid => Problems.Count == 0;
	public SaveIntegrityReport031(IEnumerable<string> problems)
	{
		Problems = (problems ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
	}
}
