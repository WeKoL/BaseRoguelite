using Godot;
using System;
using System.Collections.Generic;

public partial class SafeZoneUnloadNotification : Control
{
	[Export] public PackedScene NotificationItemScene { get; set; }
	[Export] public int LeftPadding { get; set; } = 16;
	[Export] public int TopPadding { get; set; } = 16;
	[Export] public int VerticalSpacing { get; set; } = 6;
	[Export] public double HideCascadeDelaySeconds { get; set; } = 0.35;

	private static readonly Color PickupTextColor = new Color(0.35f, 1.0f, 0.35f, 1.0f);
	private static readonly Color RemovalTextColor = new Color(1.0f, 0.35f, 0.35f, 1.0f);

	private Control _itemsRoot;
	private readonly List<SafeZoneUnloadNotificationItem> _activeItems = new();
	private double _hideCooldownSeconds;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		MouseFilter = MouseFilterEnum.Ignore;
		TopLevel = true;
		ZIndex = 100;

		_itemsRoot = GetNodeOrNull<Control>("ItemsRoot") ?? this;
	}

	public override void _Process(double delta)
	{
		if (_hideCooldownSeconds > 0.0)
			_hideCooldownSeconds = Math.Max(0.0, _hideCooldownSeconds - delta);

		if (_hideCooldownSeconds > 0.0)
			return;

		SafeZoneUnloadNotificationItem nextItem = GetNextReadyItem();
		if (nextItem == null)
			return;

		nextItem.StartSlideOut();
		_hideCooldownSeconds = HideCascadeDelaySeconds;
	}

	public void ShowPickup(ItemData item, int amount, int finalInventoryAmount)
	{
		if (item == null || amount <= 0)
			return;

		string text = InventoryNotificationTextBuilder.BuildPickup(
			ResolveDisplayName(item),
			amount,
			finalInventoryAmount);
			
		if (string.IsNullOrWhiteSpace(text))
			return;
			
		AddNotification(text, PickupTextColor);
	}

	public void ShowDrop(ItemData item, int amount, int finalInventoryAmount)
	{
		if (item == null || amount <= 0)
			return;

		string text = InventoryNotificationTextBuilder.BuildDrop(
			ResolveDisplayName(item),
			amount,
			finalInventoryAmount);

		if (string.IsNullOrWhiteSpace(text))
			return;

		AddNotification(text, RemovalTextColor);
	}

	public void ShowUnloadResult(SafeZoneUnloadResult result, Func<string, string> displayNameResolver = null)
	{
		if (result == null || !result.MovedAnything)
			return;

		if (NotificationItemScene == null)
		{
			GD.PushWarning("SafeZoneUnloadNotification: не назначена сцена NotificationItemScene.");
			return;
		}

		foreach (SafeZoneUnloadEntry entry in result.Entries)
		{
			string text = SafeZoneUnloadEntryNotificationTextBuilder.Build(entry, displayNameResolver);
			if (string.IsNullOrWhiteSpace(text))
				continue;

			AddNotification(text, RemovalTextColor);
		}
	}

	private static string ResolveDisplayName(ItemData item)
	{
		if (item == null)
			return "item";

		if (!string.IsNullOrWhiteSpace(item.DisplayName))
			return item.DisplayName;

		if (!string.IsNullOrWhiteSpace(item.Id))
			return item.Id;

		return "item";
	}

	private void AddNotification(string text, Color textColor)
	{
		SafeZoneUnloadNotificationItem item = NotificationItemScene.Instantiate<SafeZoneUnloadNotificationItem>();
		item.Expired += OnItemExpired;
		_itemsRoot.AddChild(item);
		_activeItems.Add(item);
		item.ShowNotification(text, textColor);
		RelayoutItems();
	}

	private void OnItemExpired(SafeZoneUnloadNotificationItem item)
	{
		if (item != null)
			item.Expired -= OnItemExpired;

		_activeItems.Remove(item);
		RelayoutItems();
	}

	private SafeZoneUnloadNotificationItem GetNextReadyItem()
	{
		for (int i = 0; i < _activeItems.Count; i++)
		{
			SafeZoneUnloadNotificationItem item = _activeItems[i];
			if (item == null || !IsInstanceValid(item))
				continue;
			if (!item.IsReadyToHide)
				continue;
			if (item.IsSliding)
				continue;

			return item;
		}

		return null;
	}

	private void RelayoutItems()
	{
		float y = TopPadding;

		for (int i = _activeItems.Count - 1; i >= 0; i--)
		{
			SafeZoneUnloadNotificationItem item = _activeItems[i];
			if (item == null || !IsInstanceValid(item))
			{
				_activeItems.RemoveAt(i);
				continue;
			}
		}

		foreach (SafeZoneUnloadNotificationItem item in _activeItems)
		{
			item.SetStackPosition(new Vector2(LeftPadding, y));
			y += item.GetNotificationHeight() + VerticalSpacing;
		}
	}
}
