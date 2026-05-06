public static class SaveGameMigrationService
{
	public const int CurrentVersion = 5;

	public static SaveGameData Migrate(SaveGameData data)
	{
		if (data == null) return null;

		if (data.Version < 2)
		{
			if (data.PlayerMaxStamina <= 0) data.PlayerMaxStamina = 100;
			if (data.PlayerStamina <= 0) data.PlayerStamina = data.PlayerMaxStamina;
		}

		if (data.Version < 3 && string.IsNullOrWhiteSpace(data.BuildVersion))
			data.BuildVersion = VersionInfo.Version;

		if (data.Version < 4)
		{
			if (data.PlayerMaxFood <= 0) data.PlayerMaxFood = 100;
			if (data.PlayerFood <= 0) data.PlayerFood = data.PlayerMaxFood;
			if (data.PlayerMaxWater <= 0) data.PlayerMaxWater = 100;
			if (data.PlayerWater <= 0) data.PlayerWater = data.PlayerMaxWater;
		}

		if (data.Version < 5)
		{
			if (data.PlayerMaxStamina <= 0) data.PlayerMaxStamina = 100;
			if (data.PlayerStamina <= 0) data.PlayerStamina = data.PlayerMaxStamina;
			if (data.ExpeditionElapsedSeconds < 0) data.ExpeditionElapsedSeconds = 0;
		}

		data.Version = CurrentVersion;
		data.BuildVersion = VersionInfo.Version;
		return data;
	}
}
