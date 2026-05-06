using Godot;

public partial class Main : Node2D
{
	private PlayerController _player;
	private GameMenu _gameMenu;
	private BaseRoot _baseRoot;
	private SafeZoneUnloadNotification _safeZoneUnloadNotification;
	private Label _debugStatusLabel;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;

		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;

		_player = GetNode<PlayerController>("World/Player");
		_baseRoot = GetNode<BaseRoot>("World/BaseRoot");
		_gameMenu = GetNode<GameMenu>("UI/GameMenu");

		_safeZoneUnloadNotification = GetNodeOrNull<SafeZoneUnloadNotification>("UI/SafeZoneUnloadNotification");
		if (_safeZoneUnloadNotification == null)
		{
			CanvasLayer uiLayer = GetNodeOrNull<CanvasLayer>("UI");
			PackedScene notificationScene = GD.Load<PackedScene>("res://Shared/UI/SafeZoneUnloadNotification.tscn");

			if (uiLayer != null && notificationScene != null)
			{
				_safeZoneUnloadNotification = notificationScene.Instantiate<SafeZoneUnloadNotification>();
				_safeZoneUnloadNotification.Name = "SafeZoneUnloadNotification";
				uiLayer.AddChild(_safeZoneUnloadNotification);
			}
			else
			{
				GD.PushWarning("Main: не удалось создать SafeZoneUnloadNotification.");
			}
		}

		_player.BindGameMenu(_gameMenu);
		_gameMenu.BindPlayer(_player);
		_gameMenu.BindBase(_baseRoot);

		_player.ItemPickedUp += OnPlayerItemPickedUp;
		_gameMenu.InventoryItemDropped += OnInventoryItemDropped;

		CreateVisibleVersion30Content();
		CreateDebugStatusLabel();

		_gameMenu.CloseMenu();
	}

	public override void _Process(double delta)
	{
		UpdateDebugStatusLabel();
	}

	private void CreateVisibleVersion30Content()
	{
		Node2D world = GetNodeOrNull<Node2D>("World");
		if (world == null || world.HasNode("VisibleV30Content"))
			return;

		Node2D root = new Node2D { Name = "VisibleV30Content" };
		world.AddChild(root);

		ItemCatalog catalog = _player?.ItemCatalog ?? ItemCatalog.LoadFromFolder();
		ItemData rock = catalog.GetOrFallback("rock", 99, 1.0f);
		ItemData wood = catalog.GetOrFallback("wooden_plank", 99, 1.0f);
		ItemData metal = catalog.GetOrFallback("metal", 99, 1.0f);
		ItemData food = catalog.GetOrFallback("canned_food", 8, 0.7f);
		ItemData water = catalog.GetOrFallback("water_bottle", 8, 0.6f);
		ItemData ammo = catalog.GetOrFallback("ammo_pack", 5, 1.2f);
		ItemData toolKit = catalog.GetOrFallback("tool_kit", 3, 2.0f);

		CreateResourceNode(root, new Vector2(-220f, -120f), "Каменная жила", rock, 2, 4, new Color(0.65f, 0.65f, 0.72f));
		CreateResourceNode(root, new Vector2(-270f, 80f), "Куча досок", wood, 2, 4, new Color(0.64f, 0.43f, 0.25f));
		CreateResourceNode(root, new Vector2(-360f, -20f), "Металлолом", metal, 1, 5, new Color(0.55f, 0.62f, 0.68f));
		CreateResourceNode(root, new Vector2(-430f, 140f), "Забытый ящик еды", food, 1, 3, new Color(0.75f, 0.55f, 0.25f));
		CreateResourceNode(root, new Vector2(-130f, 130f), "Запас воды", water, 1, 3, new Color(0.35f, 0.65f, 1.0f));
		CreateResourceNode(root, new Vector2(-700f, -130f), "Военный ящик 0.3", ammo, 1, 2, new Color(0.35f, 0.75f, 0.35f));
		CreateResourceNode(root, new Vector2(-760f, 160f), "Технический ящик 0.3", toolKit, 1, 1, new Color(0.9f, 0.7f, 0.25f));
		CreateDangerZone(root);
		CreateEnemy(root, new Vector2(-520f, -30f));
		CreateEnemy(root, new Vector2(-620f, 90f));
	}

	private void CreateResourceNode(Node parent, Vector2 position, string displayName, ItemData item, int amount, int charges, Color color)
	{
		WorldResourceNode node = new WorldResourceNode
		{
			Name = displayName.Replace(" ", "_"),
			Position = position,
			ResourceDisplayName = displayName,
			DropItem = item,
			DropAmountPerGather = amount,
			GatherCharges = charges,
			InteractionRadius = 86f
		};

		Sprite2D visual = new Sprite2D
		{
			Name = "Visual",
			Texture = item?.Icon ?? GD.Load<Texture2D>("res://icon.svg"),
			Scale = new Vector2(0.36f, 0.36f),
			Modulate = color
		};

		Label label = new Label
		{
			Name = "NameLabel",
			Text = displayName,
			Position = new Vector2(-58f, 32f),
			Size = new Vector2(116f, 22f),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		label.AddThemeFontSizeOverride("font_size", 11);

		node.AddChild(visual);
		node.AddChild(label);
		parent.AddChild(node);
	}

	private void CreateDangerZone(Node parent)
	{
		DangerZoneArea zone = new DangerZoneArea
		{
			Name = "VisibleDangerZone",
			Position = new Vector2(-520f, 0f),
			ZoneKind = WorldZoneKind.Far,
			DangerLevel = 3,
			DisplayName = "Опасная свалка"
		};

		CollisionShape2D shape = new CollisionShape2D
		{
			Name = "CollisionShape2D",
			Shape = new RectangleShape2D { Size = new Vector2(500f, 360f) }
		};

		Polygon2D visual = new Polygon2D
		{
			Name = "DangerVisual",
			Color = new Color(0.45f, 0.08f, 0.08f, 0.18f),
			Polygon = new Vector2[]
			{
				new Vector2(-250f, -180f),
				new Vector2(250f, -180f),
				new Vector2(250f, 180f),
				new Vector2(-250f, 180f)
			}
		};

		zone.AddChild(visual);
		zone.AddChild(shape);
		parent.AddChild(zone);
	}

	private void CreateEnemy(Node parent, Vector2 position)
	{
		SimpleEnemy enemy = new SimpleEnemy
		{
			Name = "TestMarauder",
			Position = position,
			MaxHealth = 45,
			TouchDamage = 7,
			MoveSpeed = 70f,
			DetectionRadius = 330f,
			AttackRadius = 42f,
			AttackCooldown = 1.2f
		};

		Sprite2D visual = new Sprite2D
		{
			Name = "Visual",
			Texture = GD.Load<Texture2D>("res://icon.svg"),
			Scale = new Vector2(0.28f, 0.28f),
			Modulate = new Color(1f, 0.25f, 0.25f)
		};

		Label label = new Label
		{
			Name = "NameLabel",
			Text = "Враг",
			Position = new Vector2(-32f, 28f),
			Size = new Vector2(64f, 20f),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		label.AddThemeFontSizeOverride("font_size", 11);

		enemy.AddChild(visual);
		enemy.AddChild(label);
		parent.AddChild(enemy);
	}

	private void CreateDebugStatusLabel()
	{
		CanvasLayer ui = GetNodeOrNull<CanvasLayer>("UI");
		if (ui == null || ui.HasNode("VisibleV30Status"))
			return;

		_debugStatusLabel = new Label
		{
			Name = "VisibleV30Status",
			Position = new Vector2(18f, 18f),
			Size = new Vector2(720f, 150f),
			Text = ""
		};
		_debugStatusLabel.AddThemeFontSizeOverride("font_size", 15);
		_debugStatusLabel.AddThemeColorOverride("font_color", new Color(0.92f, 0.96f, 1f));
		_debugStatusLabel.AddThemeColorOverride("font_shadow_color", Colors.Black);
		_debugStatusLabel.AddThemeConstantOverride("shadow_offset_x", 2);
		_debugStatusLabel.AddThemeConstantOverride("shadow_offset_y", 2);
		ui.AddChild(_debugStatusLabel);
	}

	private void UpdateDebugStatusLabel()
	{
		if (_debugStatusLabel == null || _player == null)
			return;

		_debugStatusLabel.Text = _player.BuildGameplayStatusText();
	}

	private void OnPlayerItemPickedUp(ItemData item, int amount, int finalInventoryAmount)
	{
		if (_safeZoneUnloadNotification == null)
			return;

		_safeZoneUnloadNotification.ShowPickup(item, amount, finalInventoryAmount);
	}

	private void OnInventoryItemDropped(ItemData item, int amount, int finalInventoryAmount)
	{
		if (_safeZoneUnloadNotification == null)
			return;

		_safeZoneUnloadNotification.ShowDrop(item, amount, finalInventoryAmount);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey keyEvent)
			return;

		if (!keyEvent.Pressed || keyEvent.Echo)
			return;

		if (@event.IsActionPressed("open_menu"))
		{
			_gameMenu.ToggleMenu();
			GetViewport().SetInputAsHandled();
			return;
		}

		if (@event.IsActionPressed("ui_cancel"))
		{
			if (_gameMenu.IsOpen)
				_gameMenu.HandleBackAction();
			else
				GetTree().Quit();

			GetViewport().SetInputAsHandled();
			return;
		}

		if (@event.IsActionPressed("quit_game"))
		{
			if (_gameMenu.IsOpen)
				_gameMenu.HandleBackAction();
			else
				GetTree().Quit();

			GetViewport().SetInputAsHandled();
		}
	}
}
