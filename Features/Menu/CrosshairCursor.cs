using Godot;

public partial class CrosshairCursor : Control
{
	[Export]
	public float HalfSize { get; set; } = 12f;

	[Export]
	public float Gap { get; set; } = 3f;

	[Export]
	public float LineWidth { get; set; } = 2f;

	[Export]
	public Color CrosshairColor { get; set; } = new Color(1f, 1f, 1f, 0.95f);

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		MouseFilter = MouseFilterEnum.Ignore;

		float size = HalfSize * 2f + 8f;
		CustomMinimumSize = new Vector2(size, size);
		Size = CustomMinimumSize;

		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		bool shouldBeVisible =
			Input.MouseMode == Input.MouseModeEnum.Hidden ||
			Input.MouseMode == Input.MouseModeEnum.ConfinedHidden ||
			Input.MouseMode == Input.MouseModeEnum.Captured;

		Visible = shouldBeVisible;

		if (!Visible)
			return;

		Vector2 mousePos = GetViewport().GetMousePosition();
		Position = mousePos - Size / 2f;
	}

	public override void _Draw()
	{
		Vector2 c = Size / 2f;

		DrawLine(c + new Vector2(-HalfSize, 0), c + new Vector2(-Gap, 0), CrosshairColor, LineWidth, true);
		DrawLine(c + new Vector2(Gap, 0), c + new Vector2(HalfSize, 0), CrosshairColor, LineWidth, true);
		DrawLine(c + new Vector2(0, -HalfSize), c + new Vector2(0, -Gap), CrosshairColor, LineWidth, true);
		DrawLine(c + new Vector2(0, Gap), c + new Vector2(0, HalfSize), CrosshairColor, LineWidth, true);
	}
}
