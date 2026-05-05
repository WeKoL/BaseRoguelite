using Godot;

public partial class WorldResourceNode : Node2D, IMenuInteractable
{
	[Export] public ItemData DropItem { get; set; }
	[Export] public int DropAmountPerGather { get; set; } = 1;
	[Export] public int GatherCharges { get; set; } = 3;
	[Export] public float InteractionRadius { get; set; } = 72f;
	[Export] public string ResourceDisplayName { get; set; } = "Ресурс";

	private Label _nameLabel;
	private float _feedbackTimer;

	public override void _Ready()
	{
		AddToGroup("menu_interactable");
		_nameLabel = GetNodeOrNull<Label>("NameLabel");
		RefreshLabel();
	}

	public override void _Process(double delta)
	{
		if (_feedbackTimer <= 0f)
			return;

		_feedbackTimer = Mathf.Max(0f, _feedbackTimer - (float)delta);
		if (_feedbackTimer <= 0f)
			Modulate = Colors.White;
	}

	public string GetInteractionText(PlayerController player)
	{
		return $"Удерживай E: добыть {ResourceDisplayName} ({GatherCharges} ост.)";
	}

	public Vector2 GetInteractionWorldPosition()
	{
		return GlobalPosition;
	}

	public float GetInteractionRadius()
	{
		return InteractionRadius;
	}

	public void SetSelectedByPlayer(bool selected)
	{
		if (_feedbackTimer > 0f)
			return;

		Modulate = selected ? new Color(1.2f, 1.2f, 1.2f) : Colors.White;
	}

	public void Interact(PlayerController player, GameMenu menu)
	{
		GatherResult result = TryGather(player);
		if (result.Succeeded)
		{
			Modulate = new Color(0.65f, 1.15f, 0.65f);
			_feedbackTimer = 0.18f;
		}
		else
		{
			Modulate = new Color(1.15f, 0.45f, 0.45f);
			_feedbackTimer = 0.28f;
		}

		if (result.Depleted)
			QueueFree();
	}

	public GatherResult TryGather(PlayerController player)
	{
		if (player == null || DropItem == null || DropAmountPerGather <= 0 || GatherCharges <= 0)
			return GatherResult.Fail("invalid_resource");

		int added = player.Inventory.TryAddItem(DropItem, DropAmountPerGather);
		if (added <= 0)
			return GatherResult.Fail("inventory_full");

		GatherCharges--;
		RefreshLabel();
		return GatherResult.Success(DropItem.Id, added, GatherCharges <= 0);
	}

	private void RefreshLabel()
	{
		if (_nameLabel == null)
			return;

		_nameLabel.Text = $"{ResourceDisplayName}\n{GatherCharges}x";
	}
}
