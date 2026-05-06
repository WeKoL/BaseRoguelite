using System;

public sealed class SaveBackupPolicy030
{
	public int MaxBackups { get; }
	public int MinimumMinutesBetweenBackups { get; }
	public SaveBackupPolicy030(int maxBackups = 5, int minimumMinutesBetweenBackups = 10)
	{
		MaxBackups = Math.Max(1, maxBackups);
		MinimumMinutesBetweenBackups = Math.Max(0, minimumMinutesBetweenBackups);
	}
	public bool ShouldCreateBackup(DateTime lastBackupUtc, DateTime nowUtc, bool importantProgress)
	{
		if (importantProgress) return true;
		return (nowUtc - lastBackupUtc).TotalMinutes >= MinimumMinutesBetweenBackups;
	}
}
