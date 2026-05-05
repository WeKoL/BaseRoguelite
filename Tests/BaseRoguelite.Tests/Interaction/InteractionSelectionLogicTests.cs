using Xunit;

namespace BaseRoguelite.Tests.Interaction;

public class InteractionSelectionLogicTests
{
	[Fact]
	public void SelectClosest_ReturnsNull_WhenNoCandidates()
	{
		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, new InteractionCandidate[0]);

		Assert.Null(result);
	}

	[Fact]
	public void SelectClosest_ReturnsNull_WhenAllCandidatesAreOutsideRadius()
	{
		object a = new object();
		object b = new object();

		var candidates = new[]
		{
			new InteractionCandidate(a, 10f, 0f, 2f),
			new InteractionCandidate(b, -8f, 0f, 1f)
		};

		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, candidates);

		Assert.Null(result);
	}

	[Fact]
	public void SelectClosest_ReturnsCandidate_WhenInsideRadius()
	{
		object target = new object();

		var candidates = new[]
		{
			new InteractionCandidate(target, 1f, 1f, 5f)
		};

		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, candidates);

		Assert.Same(target, result);
	}

	[Fact]
	public void SelectClosest_ReturnsNearestCandidate_WhenSeveralAreInsideRadius()
	{
		object near = new object();
		object far = new object();

		var candidates = new[]
		{
			new InteractionCandidate(far, 4f, 0f, 10f),
			new InteractionCandidate(near, 1f, 0f, 10f)
		};

		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, candidates);

		Assert.Same(near, result);
	}

	[Fact]
	public void SelectClosest_IgnoresCandidate_WithNonPositiveRadius()
	{
		object invalid = new object();
		object valid = new object();

		var candidates = new[]
		{
			new InteractionCandidate(invalid, 0f, 0f, 0f),
			new InteractionCandidate(valid, 2f, 0f, 3f)
		};

		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, candidates);

		Assert.Same(valid, result);
	}

	[Fact]
	public void SelectClosest_KeepsFirstCandidate_WhenDistancesAreEqual()
	{
		object first = new object();
		object second = new object();

		var candidates = new[]
		{
			new InteractionCandidate(first, 1f, 0f, 5f),
			new InteractionCandidate(second, -1f, 0f, 5f)
		};

		object result = InteractionSelectionLogic.SelectClosest(0f, 0f, candidates);

		Assert.Same(first, result);
	}
}
