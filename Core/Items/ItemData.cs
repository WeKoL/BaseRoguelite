using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
	[Export] public string Id { get; set; } = string.Empty;
	[Export] public string DisplayName { get; set; } = string.Empty;
	[Export(PropertyHint.MultilineText)] public string Description { get; set; } = string.Empty;
	[Export] public ItemCategory Category { get; set; } = ItemCategory.Material;
	[Export] public ItemRarity Rarity { get; set; } = ItemRarity.Common;
	[Export] public int MaxStackSize { get; set; } = 99;
	[Export] public float Weight { get; set; } = 1.0f;
	[Export] public string UsageHint { get; set; } = string.Empty;
	[Export] public Texture2D Icon { get; set; }

	[Export] public bool IsConsumable { get; set; } = false;
	[Export] public int HealthRestore { get; set; } = 0;
	[Export] public int FoodRestore { get; set; } = 0;
	[Export] public int WaterRestore { get; set; } = 0;
	[Export] public int StaminaRestore { get; set; } = 0;

	[Export] public bool IsEquippable { get; set; } = false;
	[Export] public EquipmentSlotId EquipSlotId { get; set; } = EquipmentSlotId.Head;
	[Export] public int EquipmentMaxDurability { get; set; } = 100;

	public bool CanUse()
	{
		return IsConsumable && (HealthRestore > 0 || FoodRestore > 0 || WaterRestore > 0 || StaminaRestore > 0);
	}

	public bool CanEquip()
	{
		return IsEquippable && EquipmentMaxDurability > 0;
	}

	public string GetDisplayCategory()
	{
		return Category switch
		{
			ItemCategory.Material => "Материал",
			ItemCategory.Product => "Изделие",
			ItemCategory.Weapon => "Оружие",
			_ => "Предмет"
		};
	}
}
