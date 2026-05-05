public static class SaveGameMigrationService
{
	public const int CurrentVersion = 3;
	public static SaveGameData Migrate(SaveGameData data)
	{
		if (data == null) return null;
		if (data.Version < 2) { if (data.PlayerMaxStamina <= 0) data.PlayerMaxStamina = 100; if (data.PlayerStamina <= 0) data.PlayerStamina = data.PlayerMaxStamina; }
		if (data.Version < 3 && string.IsNullOrWhiteSpace(data.BuildVersion)) data.BuildVersion = VersionInfo.Version;
		data.Version = CurrentVersion; data.BuildVersion = VersionInfo.Version; return data;
	}
}
