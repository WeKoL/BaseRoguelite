using Godot;
using System.Collections.Generic;

public sealed partial class ItemCatalog : RefCounted
{
	private readonly Dictionary<string, ItemData> _items = new();
	public IReadOnlyDictionary<string, ItemData> Items => _items;
	public int Count => _items.Count;
	public void Register(ItemData item)
	{
		if (item == null || string.IsNullOrWhiteSpace(item.Id)) return;
		_items[item.Id] = item;
	}
	public bool TryGet(string itemId, out ItemData item)
	{
		item = null;
		return !string.IsNullOrWhiteSpace(itemId) && _items.TryGetValue(itemId, out item);
	}
	public ItemData GetOrFallback(string itemId, int maxStackSize = 99, float weight = 1f)
	{
		if (TryGet(itemId, out ItemData item)) return item;
		return new ItemData{Id=itemId??"",DisplayName=string.IsNullOrWhiteSpace(itemId)?"unknown_item":itemId,Description="Fallback из ItemCatalog",MaxStackSize=maxStackSize<=0?99:maxStackSize,Weight=weight<=0f?1f:weight};
	}
	public static ItemCatalog LoadFromFolder(string rootPath="res://Content/Items")
	{
		ItemCatalog catalog = new();
		LoadFolderRecursive(rootPath, catalog);
		return catalog;
	}
	private static void LoadFolderRecursive(string folderPath, ItemCatalog catalog)
	{
		DirAccess dir = DirAccess.Open(folderPath);
		if (dir == null) return;
		foreach (string file in dir.GetFiles())
		{
			string name = file.EndsWith(".remap") ? file.Substring(0, file.Length - 6) : file;
			if (!name.EndsWith(".tres") && !name.EndsWith(".res")) continue;
			catalog.Register(GD.Load<ItemData>($"{folderPath}/{name}"));
		}
		foreach (string child in dir.GetDirectories())
		{
			if (!child.StartsWith(".")) LoadFolderRecursive($"{folderPath}/{child}", catalog);
		}
	}
}
