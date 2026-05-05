using System.Collections.Generic;

public static class InteractionSelectionLogic
{
	public static object SelectClosest(float playerX, float playerY, IEnumerable<InteractionCandidate> candidates)
	{
		if (candidates == null)
			return null;

		object bestTarget = null;
		float bestDistanceSq = float.MaxValue;

		foreach (InteractionCandidate candidate in candidates)
		{
			if (candidate == null || candidate.Target == null)
				continue;

			if (candidate.Radius <= 0f)
				continue;

			float distanceSq = candidate.DistanceSquaredTo(playerX, playerY);
			float radiusSq = candidate.Radius * candidate.Radius;

			if (distanceSq > radiusSq)
				continue;

			if (distanceSq < bestDistanceSq)
			{
				bestDistanceSq = distanceSq;
				bestTarget = candidate.Target;
			}
		}

		return bestTarget;
	}
}
