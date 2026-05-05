using Godot;

public partial class InteractionProgressRing : Node2D
{
	[Export]
	public float Radius { get; set; } = 28f;

	[Export]
	public float RingWidth { get; set; } = 6f;

	[Export]
	public int SegmentCount { get; set; } = 72;

	[Export]
	public Color BackgroundColor { get; set; } = new Color(0.45f, 0.45f, 0.45f, 0.95f);

	private bool _isActive;
	private float _progress;

	public override void _Ready()
	{
		ZIndex = 100;
		Visible = false;
	}

	public void SetProgressState(bool isActive, float progress)
	{
		_isActive = isActive;
		_progress = Mathf.Clamp(progress, 0f, 1f);

		Visible = _isActive;
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (!_isActive)
			return;

		float startAngle = -Mathf.Pi / 2.0f;
		float fullAngle = Mathf.Tau;

		DrawArc(
			Vector2.Zero,
			Radius,
			startAngle,
			startAngle + fullAngle,
			SegmentCount,
			BackgroundColor,
			RingWidth,
			true
		);

		if (_progress <= 0.001f)
			return;

		int filledSegments = Mathf.Max(1, Mathf.CeilToInt(SegmentCount * _progress));
		float segmentAngle = fullAngle / SegmentCount;

		for (int i = 0; i < filledSegments; i++)
		{
			float t = filledSegments <= 1 ? 1f : (float)i / (filledSegments - 1);

			float segStart = startAngle + i * segmentAngle;
			float segEnd = segStart + segmentAngle;

			DrawArc(
				Vector2.Zero,
				Radius,
				segStart,
				segEnd,
				2,
				GetProgressColor(t),
				RingWidth,
				true
			);
		}
	}

	private Color GetProgressColor(float t)
	{
		float hue = Mathf.Lerp(0.0f, 0.33333334f, Mathf.Clamp(t, 0f, 1f));
		return Color.FromHsv(hue, 0.95f, 1.0f, 1.0f);
	}
}
