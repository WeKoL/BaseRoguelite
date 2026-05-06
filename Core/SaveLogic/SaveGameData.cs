using System.Collections.Generic;

public sealed class SaveGameData
{
	public int Version { get; set; } = 5;
	public string BuildVersion { get; set; } = VersionInfo.Version;
	public int PlayerHealth { get; set; }
	public int PlayerMaxHealth { get; set; }
	public int PlayerStamina { get; set; }
	public int PlayerMaxStamina { get; set; }
	public int PlayerFood { get; set; } = 100;
	public int PlayerMaxFood { get; set; } = 100;
	public int PlayerWater { get; set; } = 100;
	public int PlayerMaxWater { get; set; } = 100;
	public int BaseLevel { get; set; }
	public int ExpeditionElapsedSeconds { get; set; }
	public List<SaveSlotItemData> InventoryItems { get; set; } = new();
	public List<SaveSlotItemData> StorageItems { get; set; } = new();
	public List<SaveEquippedItemData> EquipmentItems { get; set; } = new();
	public List<SaveQuestProgressData> QuestProgress { get; set; } = new();
}
