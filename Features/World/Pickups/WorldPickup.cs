using Godot;

public partial class WorldPickup : Area2D
{
	[Export]
	public ItemData Item { get; set; }

	[Export]
	public int Amount { get; set; } = 1;

	private CollisionShape2D _collisionShape;
	private Sprite2D _iconSprite;
	private Label _amountLabel;
	private bool _isCollected;

	public bool IsAvailable =>
		!_isCollected &&
		Item != null &&
		Amount > 0 &&
		IsInsideTree() &&
		!IsQueuedForDeletion();

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Pausable;

		AddToGroup("world_pickups");

		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		_iconSprite = GetNodeOrNull<Sprite2D>("Icon");
		_amountLabel = GetNodeOrNull<Label>("AmountLabel");

		RefreshVisuals();
	}

	public bool TryPickup(PlayerInventoryState inventory)
	{
		if (!IsAvailable)
			return false;

		if (inventory == null)
			return false;

		PickupResult result = inventory.TryPickupItem(Item, Amount);
		if (!result.Succeeded)
			return false;

		BeginCollected();
		return true;
	}

	private void BeginCollected()
	{
		if (_isCollected)
			return;

		_isCollected = true;

		RemoveFromGroup("world_pickups");

		Monitoring = false;
		Monitorable = false;

		if (_collisionShape != null)
			_collisionShape.SetDeferred("disabled", true);

		Visible = false;
		SetProcess(false);
		SetPhysicsProcess(false);
		SetProcessInput(false);

		QueueFree();
	}

	private void RefreshVisuals()
	{
		if (_iconSprite != null)
		{
			_iconSprite.Texture = Item?.Icon;
			_iconSprite.Visible = _iconSprite.Texture != null;
		}

		if (_amountLabel != null)
		{
			_amountLabel.Text = Amount > 0 ? $"x{Amount}" : string.Empty;
			_amountLabel.Visible = !string.IsNullOrWhiteSpace(_amountLabel.Text);
		}
	}
}
