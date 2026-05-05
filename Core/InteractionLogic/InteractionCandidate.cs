public sealed class InteractionCandidate
{
	public object Target { get; }
	public float X { get; }
	public float Y { get; }
	public float Radius { get; }

	public InteractionCandidate(object target, float x, float y, float radius)
	{
		Target = target;
		X = x;
		Y = y;
		Radius = radius;
	}

	public float DistanceSquaredTo(float px, float py)
	{
		float dx = px - X;
		float dy = py - Y;
		return dx * dx + dy * dy;
	}
}
