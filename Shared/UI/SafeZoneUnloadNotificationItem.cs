using Godot;
using System;

public partial class SafeZoneUnloadNotificationItem : PanelContainer
{
	[Export] public double VisibleDurationSeconds { get; set; } = 5.0;
	[Export] public double SlideDurationSeconds { get; set; } = 1.2;
	[Export] public float OffscreenPadding { get; set; } = 48.0f;

	private Label _messageLabel;
	private double _aliveSeconds;
	private double _slideElapsed;
	private float _slideStartX;
	private float _slideTargetX;
	private bool _isSliding;

	public bool IsSliding => _isSliding;
	public bool IsReadyToHide => !_isSliding && _aliveSeconds >= VisibleDurationSeconds;

	public event Action<SafeZoneUnloadNotificationItem> Expired;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		MouseFilter = MouseFilterEnum.Ignore;
		_messageLabel = GetNode<Label>("Padding/MessageLabel");
		Visible = false;
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		if (!_isSliding)
		{
			_aliveSeconds += delta;
			return;
		}

		_slideElapsed += delta;
		float t = (float)Math.Min(1.0, _slideElapsed / Math.Max(0.01, SlideDurationSeconds));
		Position = new Vector2(Mathf.Lerp(_slideStartX, _slideTargetX, t), Position.Y);

		if (t < 1.0f)
			return;

		SetProcess(false);
		Expired?.Invoke(this);
		QueueFree();
	}

	public void ShowNotification(string text, Color textColor)
	{
		if (string.IsNullOrWhiteSpace(text))
			return;

		_messageLabel.Text = text;
		_messageLabel.AddThemeColorOverride("font_color", textColor);

		_aliveSeconds = 0.0;
		_slideElapsed = 0.0;
		_isSliding = false;
		Visible = true;
		SetProcess(true);
	}

	public void SetStackPosition(Vector2 position)
	{
		Position = new Vector2(_isSliding ? Position.X : position.X, position.Y);
	}

	public float GetNotificationHeight()
	{
		float height = Size.Y;
		if (height <= 0.0f)
			height = CustomMinimumSize.Y;
		if (height <= 0.0f)
			height = 28.0f;

		return height;
	}

	public void StartSlideOut()
	{
		if (_isSliding)
			return;

		_isSliding = true;
		_slideElapsed = 0.0;
		_slideStartX = Position.X;

		float width = Size.X > 0.0f
			? Size.X
			: (CustomMinimumSize.X > 0.0f ? CustomMinimumSize.X : 220.0f);

		_slideTargetX = -width - OffscreenPadding;
	}
}
