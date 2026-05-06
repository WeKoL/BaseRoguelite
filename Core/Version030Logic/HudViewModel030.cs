public sealed class HudViewModel030
{
	public string HealthText { get; }
	public string NeedsText { get; }
	public string ZoneText { get; }
	public string WarningText { get; }
	public HudViewModel030(string healthText, string needsText, string zoneText, string warningText)
	{
		HealthText = healthText ?? string.Empty;
		NeedsText = needsText ?? string.Empty;
		ZoneText = zoneText ?? string.Empty;
		WarningText = warningText ?? string.Empty;
	}
}

public static class HudViewModelBuilder030
{
	public static HudViewModel030 Build(PlayerStatsState stats, SurvivalNeedsState needs, WorldZoneState zone)
	{
		string health = stats == null ? "HP: ?" : $"HP: {stats.CurrentHealth}/{stats.MaxHealth} | STA: {stats.CurrentStamina}/{stats.MaxStamina}";
		string needText = needs == null ? "Еда/вода: ?" : $"Еда: {needs.Food}/{needs.MaxFood} | Вода: {needs.Water}/{needs.MaxWater}";
		string zoneText = zone == null ? "Зона: обычная" : $"Зона: {zone.DisplayName} | опасность {zone.DangerLevel}";
		string warning = needs != null && needs.IsCritical ? "Критический голод или жажда" : stats != null && stats.IsDead ? "Игрок погиб" : string.Empty;
		return new HudViewModel030(health, needText, zoneText, warning);
	}
}
