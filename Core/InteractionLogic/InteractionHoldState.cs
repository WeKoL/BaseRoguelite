using System;

public sealed class InteractionHoldState
{
	public float HoldTime { get; private set; }
	public float Progress { get; private set; }

	public InteractionHoldUpdateResult Update(bool hasTarget, bool isPressed, float delta, float holdDuration)
	{
		if (!hasTarget || !isPressed)
		{
			Reset();
			return new InteractionHoldUpdateResult(false, 0f, false);
		}

		if (holdDuration <= 0f)
		{
			Reset();
			return new InteractionHoldUpdateResult(false, 1f, true);
		}

		if (delta < 0f)
			delta = 0f;

		HoldTime += delta;
		Progress = (float)Math.Clamp(HoldTime / holdDuration, 0.0, 1.0);

		if (Progress >= 1f)
		{
			Reset();
			return new InteractionHoldUpdateResult(false, 1f, true);
		}

		return new InteractionHoldUpdateResult(true, Progress, false);
	}

	public void Reset()
	{
		HoldTime = 0f;
		Progress = 0f;
	}
}
