using Xunit;

namespace BaseRoguelite.Tests.Interaction;

public class InteractionHoldStateTests
{
	[Fact]
	public void NewState_StartsAtZero()
	{
		var state = new InteractionHoldState();

		Assert.Equal(0f, state.HoldTime);
		Assert.Equal(0f, state.Progress);
	}

	[Fact]
	public void Update_WithoutTarget_StaysReset()
	{
		var state = new InteractionHoldState();

		var result = state.Update(hasTarget: false, isPressed: true, delta: 0.2f, holdDuration: 0.6f);

		Assert.False(result.IsActive);
		Assert.False(result.JustCompleted);
		Assert.Equal(0f, result.Progress);
		Assert.Equal(0f, state.HoldTime);
		Assert.Equal(0f, state.Progress);
	}

	[Fact]
	public void Update_WithoutButton_StaysReset()
	{
		var state = new InteractionHoldState();

		var result = state.Update(hasTarget: true, isPressed: false, delta: 0.2f, holdDuration: 0.6f);

		Assert.False(result.IsActive);
		Assert.False(result.JustCompleted);
		Assert.Equal(0f, result.Progress);
		Assert.Equal(0f, state.HoldTime);
		Assert.Equal(0f, state.Progress);
	}

	[Fact]
	public void Update_AccumulatesProgress_WhileHeld()
	{
		var state = new InteractionHoldState();

		var result = state.Update(hasTarget: true, isPressed: true, delta: 0.3f, holdDuration: 0.6f);

		Assert.True(result.IsActive);
		Assert.False(result.JustCompleted);
		Assert.Equal(0.5f, result.Progress, 3);
		Assert.Equal(0.3f, state.HoldTime, 3);
		Assert.Equal(0.5f, state.Progress, 3);
	}

	[Fact]
	public void Update_ReleaseResetsProgress()
	{
		var state = new InteractionHoldState();

		state.Update(hasTarget: true, isPressed: true, delta: 0.3f, holdDuration: 0.6f);
		var result = state.Update(hasTarget: true, isPressed: false, delta: 0.1f, holdDuration: 0.6f);

		Assert.False(result.IsActive);
		Assert.False(result.JustCompleted);
		Assert.Equal(0f, result.Progress);
		Assert.Equal(0f, state.HoldTime);
		Assert.Equal(0f, state.Progress);
	}

	[Fact]
	public void Update_Completes_WhenThresholdReached()
	{
		var state = new InteractionHoldState();

		state.Update(hasTarget: true, isPressed: true, delta: 0.3f, holdDuration: 0.6f);
		var result = state.Update(hasTarget: true, isPressed: true, delta: 0.3f, holdDuration: 0.6f);

		Assert.False(result.IsActive);
		Assert.True(result.JustCompleted);
		Assert.Equal(1f, result.Progress, 3);
		Assert.Equal(0f, state.HoldTime);
		Assert.Equal(0f, state.Progress);
	}

	[Fact]
	public void Update_CanStartAgain_AfterCompletion()
	{
		var state = new InteractionHoldState();

		state.Update(hasTarget: true, isPressed: true, delta: 0.6f, holdDuration: 0.6f);
		var result = state.Update(hasTarget: true, isPressed: true, delta: 0.3f, holdDuration: 0.6f);

		Assert.True(result.IsActive);
		Assert.False(result.JustCompleted);
		Assert.Equal(0.5f, result.Progress, 3);
	}
}
