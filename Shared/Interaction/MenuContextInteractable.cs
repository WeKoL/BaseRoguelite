using Godot;

public partial class MenuContextInteractable : Area2D, IMenuInteractable
{
	[Export]
	public MenuSection TargetSection { get; set; } = MenuSection.Base;

	[Export]
	public BaseActionId FocusedBaseAction { get; set; } = BaseActionId.None;

	[Export]
	public string SourceId { get; set; } = "world_object";

	[Export]
	public string InteractionText { get; set; } = "E — взаимодействовать";

	[Export]
	public float InteractionRadius { get; set; } = 56f;

	[Export]
	public float InteractionRingWidth { get; set; } = 3f;

	[Export]
	public Color DefaultRingColor { get; set; } = new Color(0.75f, 0.75f, 0.75f, 0.35f);

	[Export]
	public Color SelectedRingColor { get; set; } = new Color(1f, 1f, 1f, 0.95f);

	private CollisionShape2D _collisionShape;
	private bool _isSelectedByPlayer;

	public override void _Ready()
	{
		AddToGroup("menu_interactable");

		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		SyncCollisionRadius();

		QueueRedraw();
	}

	public override void _Draw()
	{
		Color ringColor = _isSelectedByPlayer ? SelectedRingColor : DefaultRingColor;

		DrawArc(
			Vector2.Zero,
			InteractionRadius,
			0f,
			Mathf.Tau,
			72,
			ringColor,
			InteractionRingWidth,
			true
		);
	}

	private void SyncCollisionRadius()
	{
		if (_collisionShape == null)
			return;

		if (_collisionShape.Shape is CircleShape2D circle)
		{
			circle.Radius = InteractionRadius;
			return;
		}

		CircleShape2D newCircle = new CircleShape2D();
		newCircle.Radius = InteractionRadius;
		_collisionShape.Shape = newCircle;
	}

	public void Interact(PlayerController player, GameMenu gameMenu)
	{
		if (player == null || gameMenu == null)
			return;

		gameMenu.OpenWithContext(new GameMenuContext
		{
			Section = TargetSection,
			FocusedBaseAction = FocusedBaseAction,
			SourceId = SourceId
		});
	}

	public string GetInteractionText(PlayerController player)
	{
		return InteractionText;
	}

	public Vector2 GetInteractionWorldPosition()
	{
		return GlobalPosition;
	}

	public float GetInteractionRadius()
	{
		return InteractionRadius;
	}

	public void SetSelectedByPlayer(bool isSelected)
	{
		if (_isSelectedByPlayer == isSelected)
			return;

		_isSelectedByPlayer = isSelected;
		QueueRedraw();
	}
}
