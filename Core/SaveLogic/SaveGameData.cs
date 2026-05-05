using System.Collections.Generic;
public sealed class SaveGameData
{
	public int Version { get; set; } = 3;
	public string BuildVersion { get; set; } = VersionInfo.Version;
	public int PlayerHealth { get; set; }
	public int PlayerMaxHealth { get; set; }
	public int PlayerStamina { get; set; }
	public int PlayerMaxStamina { get; set; }
	public int BaseLevel { get; set; }
	public int ExpeditionElapsedSeconds { get; set; }
	public List<SaveSlotItemData> InventoryItems { get; set; } = new();
	public List<SaveSlotItemData> StorageItems { get; set; } = new();
	public List<SaveEquippedItemData> EquipmentItems { get; set; } = new();
	public List<SaveQuestProgressData> QuestProgress { get; set; } = new();
}
