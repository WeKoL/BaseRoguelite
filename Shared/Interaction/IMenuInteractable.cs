using Godot;

public interface IMenuInteractable
{
	void Interact(PlayerController player, GameMenu gameMenu);
	string GetInteractionText(PlayerController player);
	Vector2 GetInteractionWorldPosition();
	float GetInteractionRadius();
	void SetSelectedByPlayer(bool isSelected);
}
