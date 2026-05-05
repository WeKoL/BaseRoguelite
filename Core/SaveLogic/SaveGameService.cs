using System; using System.IO; using System.Text.Json;
public static class SaveGameService
{
	private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
	public static bool TrySaveToFile(SaveGameData data, string path)
	{
		if (data == null || string.IsNullOrWhiteSpace(path)) return false; SaveValidationResult validation = SaveGameValidator.Validate(data); if (!validation.IsValid) return false;
		try { string dir = Path.GetDirectoryName(path); if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir); File.WriteAllText(path, JsonSerializer.Serialize(data, Options)); return true; } catch (Exception) { return false; }
	}
	public static bool TryLoadFromFile(string path, out SaveGameData data)
	{
		data = null; if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return false;
		try { data = JsonSerializer.Deserialize<SaveGameData>(File.ReadAllText(path), Options); data = SaveGameMigrationService.Migrate(data); return SaveGameValidator.Validate(data).IsValid; } catch (Exception) { data = null; return false; }
	}
}
