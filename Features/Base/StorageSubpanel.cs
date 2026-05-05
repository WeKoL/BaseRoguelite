using Godot;
using System;
using System.Collections.Generic;

public partial class StorageSubpanel : Control
{
	[Export] public PackedScene StorageCategorySectionScene { get; set; }
	[Export] public PackedScene ItemCardScene { get; set; }

	private VBoxContainer _sectionsRoot;
	private readonly Dictionary<ItemCategory, StorageCategorySection> _sections = new();
	private IReadOnlyList<ItemEntry> _pendingEntries = Array.Empty<ItemEntry>();
	private bool _isInitialized = false;

	public override void _Ready()
	{
		_sectionsRoot = FindChild("SectionsRoot", true, false) as VBoxContainer;

		if (_sectionsRoot == null)
		{
			GD.PushError("StorageSubpanel: не найден узел SectionsRoot ни по одному из вариантов.");
			return;
		}

		if (StorageCategorySectionScene == null)
		{
			GD.PushError("StorageSubpanel: не назначена сцена StorageCategorySectionScene.");
			return;
		}

		if (ItemCardScene == null)
		{
			GD.PushWarning("StorageSubpanel: ItemCardScene не назначена. Каркас секций создастся, но карточки предметов не будут добавляться.");
		}

		_isInitialized = true;

		BuildEmptySections();
		SetEntries(_pendingEntries);
	}

	public void SetEntries(IReadOnlyList<ItemEntry> entries)
	{
		_pendingEntries = entries ?? Array.Empty<ItemEntry>();

		if (!_isInitialized)
			return;

		Dictionary<ItemCategory, List<ItemEntry>> grouped = GroupEntriesByCategory(_pendingEntries);

		foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
		{
			List<ItemEntry> categoryEntries = grouped.TryGetValue(category, out List<ItemEntry> list)
				? list
				: new List<ItemEntry>();

			if (_sections.TryGetValue(category, out StorageCategorySection section))
				section.SetEntries(categoryEntries);
		}
	}

	private void BuildEmptySections()
	{
		foreach (Node child in _sectionsRoot.GetChildren())
			child.QueueFree();

		_sections.Clear();

		foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
		{
			StorageCategorySection section = StorageCategorySectionScene.Instantiate<StorageCategorySection>();
			section.ItemCardScene = ItemCardScene;

			_sectionsRoot.AddChild(section);

			section.SetSectionTitle(GetCategoryTitle(category));
			section.SetEntries(Array.Empty<ItemEntry>());

			_sections.Add(category, section);
		}
	}

	private Dictionary<ItemCategory, List<ItemEntry>> GroupEntriesByCategory(IReadOnlyList<ItemEntry> entries)
	{
		var result = new Dictionary<ItemCategory, List<ItemEntry>>();

		if (entries == null)
			return result;

		foreach (ItemEntry entry in entries)
		{
			if (entry == null || entry.IsEmpty() || entry.Item == null)
				continue;

			ItemCategory category = entry.Item.Category;

			if (!result.ContainsKey(category))
				result[category] = new List<ItemEntry>();

			result[category].Add(entry);
		}

		return result;
	}

	private string GetCategoryTitle(ItemCategory category)
	{
		return category switch
		{
			ItemCategory.Material => "Материалы",
			ItemCategory.Product => "Изделия",
			ItemCategory.Weapon => "Оружие",
			_ => "Прочее"
		};
	}
}
