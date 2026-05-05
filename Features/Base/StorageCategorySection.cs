using Godot;
using System.Collections.Generic;

public partial class StorageCategorySection : VBoxContainer
{
	[Export] public PackedScene ItemCardScene { get; set; }

	private Button _headerButton;
	private MarginContainer _bodyMargin;
	private Label _emptyLabel;
	private GridContainer _itemsGrid;

	private bool _isInitialized = false;
	private bool _isExpanded = true;
	private string _sectionTitle = "Раздел";
	private int _itemCount = 0;

	public override void _Ready()
	{
		_headerButton = GetNode<Button>("HeaderButton");
		_bodyMargin = GetNode<MarginContainer>("BodyMargin");
		_emptyLabel = GetNode<Label>("BodyMargin/BodyVBox/EmptyLabel");
		_itemsGrid = GetNode<GridContainer>("BodyMargin/BodyVBox/ItemsGrid");

		_headerButton.Pressed += OnHeaderPressed;

		_isInitialized = true;

		ApplyExpandedState();
		UpdateHeaderText();
		UpdateEmptyState();
	}

	public void SetSectionTitle(string title)
	{
		_sectionTitle = title;

		if (_isInitialized)
			UpdateHeaderText();
	}

	public void SetEntries(IReadOnlyList<ItemEntry> entries)
	{
		if (!_isInitialized)
			return;

		ClearGrid();
		_itemCount = 0;

		if (entries != null)
		{
			foreach (ItemEntry entry in entries)
			{
				if (entry == null || entry.IsEmpty() || entry.Item == null)
					continue;

				if (ItemCardScene == null)
				{
					GD.PushWarning("StorageCategorySection: ItemCardScene не назначена.");
					break;
				}

				ItemCard card = ItemCardScene.Instantiate<ItemCard>();
				card.SetItemEntry(entry);
				_itemsGrid.AddChild(card);
				_itemCount++;
			}
		}

		UpdateEmptyState();
		UpdateHeaderText();
	}

	private void OnHeaderPressed()
	{
		_isExpanded = !_isExpanded;
		ApplyExpandedState();
		UpdateHeaderText();
	}

	private void ApplyExpandedState()
	{
		if (!_isInitialized)
			return;

		_bodyMargin.Visible = _isExpanded;
	}

	private void UpdateHeaderText()
	{
		if (!_isInitialized)
			return;

		string marker = _isExpanded ? "▼" : "▶";
		_headerButton.Text = $"{marker} {_sectionTitle} ({_itemCount})";
	}

	private void UpdateEmptyState()
	{
		if (!_isInitialized)
			return;

		_emptyLabel.Visible = _itemCount == 0;
	}

	private void ClearGrid()
	{
		if (!_isInitialized)
			return;

		foreach (Node child in _itemsGrid.GetChildren())
			child.QueueFree();
	}
}
