using Godot;

public partial class SimpleEnemy : CharacterBody2D
{
	[Export] public int MaxHealth { get; set; } = 40;
	[Export] public int TouchDamage { get; set; } = 10;
	[Export] public float MoveSpeed { get; set; } = 90f;
	[Export] public float DetectionRadius { get; set; } = 260f;
	[Export] public float AttackRadius { get; set; } = 38f;
	[Export] public float AttackCooldown { get; set; } = 1f;

	private int _hp;
	private float _attackTimer;
	private float _hitFlashTimer;
	private PlayerController _target;
	private CanvasItem _visual;

	public int CurrentHealth => _hp;
	public bool IsAlive => _hp > 0 && !IsQueuedForDeletion();

	public override void _Ready()
	{
		_hp = MaxHealth;
		_visual = GetNodeOrNull<CanvasItem>("Visual");
		AddToGroup("enemies");
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		_attackTimer = Mathf.Max(0f, _attackTimer - dt);
		UpdateHitFlash(dt);
		FindTarget();

		if (_target == null || !IsAlive)
		{
			Velocity = Vector2.Zero;
			return;
		}

		float distance = GlobalPosition.DistanceTo(_target.GlobalPosition);
		if (distance > DetectionRadius)
		{
			Velocity = Vector2.Zero;
		}
		else if (distance <= AttackRadius)
		{
			Velocity = Vector2.Zero;
			TryAttack();
		}
		else
		{
			Velocity = (_target.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
		}

		MoveAndSlide();
	}

	public int TakeDamage(int damage)
	{
		if (damage <= 0 || _hp <= 0)
			return 0;

		int before = _hp;
		_hp = Mathf.Max(0, _hp - damage);
		_hitFlashTimer = 0.12f;

		if (_hp <= 0)
		{
			QueueFree();
		}

		return before - _hp;
	}

	private void TryAttack()
	{
		if (_attackTimer > 0f || _target == null)
			return;

		_attackTimer = AttackCooldown;
		_target.ApplyDamage(TouchDamage);
	}

	private void FindTarget()
	{
		if (_target != null && GodotObject.IsInstanceValid(_target))
			return;

		foreach (Node node in GetTree().GetNodesInGroup("player"))
		{
			if (node is PlayerController player)
			{
				_target = player;
				return;
			}
		}
	}

	private void UpdateHitFlash(float delta)
	{
		if (_visual == null)
			return;

		if (_hitFlashTimer > 0f)
		{
			_hitFlashTimer = Mathf.Max(0f, _hitFlashTimer - delta);
			_visual.Modulate = new Color(1.4f, 1.4f, 1.4f);
		}
		else
		{
			_visual.Modulate = new Color(1f, 0.25f, 0.25f);
		}
	}
}
