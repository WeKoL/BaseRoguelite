public static class BaseFacilityEffectCalculator
{
	public static int GetStorageEntryLimit(int storageLevel) => 20 + System.Math.Max(0, storageLevel) * 10;
	public static int GetHealingAmount(int medicalStationLevel) => 20 + System.Math.Max(0, medicalStationLevel) * 15;
	public static int GetCraftSpeedBonusPercent(int workbenchLevel) => System.Math.Max(0, workbenchLevel) * 5;
}
