public sealed class InteractionHoldUpdateResult
{
	public bool IsActive { get; }
	public float Progress { get; }
	public bool JustCompleted { get; }

	public InteractionHoldUpdateResult(bool isActive, float progress, bool justCompleted)
	{
		IsActive = isActive;
		Progress = progress;
		JustCompleted = justCompleted;
	}
}
