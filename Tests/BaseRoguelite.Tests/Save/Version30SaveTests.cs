using Xunit;

public sealed class Version30SaveTests
{
	[Fact]
	public void Validator_FailsInvalidHealth()
	{
		SaveGameData data = new() { PlayerMaxHealth = 10, PlayerHealth = 15 };
		SaveValidationResult result = SaveGameValidator.Validate(data);
		Assert.False(result.IsValid);
		Assert.Contains("invalid_player_health", result.Errors);
	}

	[Fact]
	public void Migration_UpgradesOldSaveToCurrentVersion()
	{
		SaveGameData data = new() { Version = 1, PlayerMaxHealth = 10, PlayerHealth = 10 };
		SaveGameData migrated = SaveGameMigrationService.Migrate(data);
		Assert.Equal(3, migrated.Version);
		Assert.Equal(100, migrated.PlayerMaxStamina);
		Assert.Equal(VersionInfo.Version, migrated.BuildVersion);
	}

	[Fact]
	public void SlotNameBuilder_ClampsInvalidIndex()
	{
		SaveSlotDescriptor descriptor = SaveSlotNameBuilder.Build(0);
		Assert.Equal(1, descriptor.SlotIndex);
		Assert.Equal("save_1.json", descriptor.FileName);
	}
}
