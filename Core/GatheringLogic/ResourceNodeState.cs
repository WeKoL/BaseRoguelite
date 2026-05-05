public sealed class ResourceNodeState
{
	public string DropItemId { get; }
	public int DropAmountPerGather { get; }
	public int ChargesLeft { get; private set; }
	public string RequiredToolItemId { get; }
	public bool IsDepleted => ChargesLeft <= 0;
	public bool RequiresTool => !string.IsNullOrWhiteSpace(RequiredToolItemId);
	public ResourceNodeState(string dropItemId, int dropAmountPerGather, int charges, string requiredToolItemId = "") { DropItemId = dropItemId ?? string.Empty; DropAmountPerGather = dropAmountPerGather <= 0 ? 1 : dropAmountPerGather; ChargesLeft = charges <= 0 ? 1 : charges; RequiredToolItemId = requiredToolItemId ?? string.Empty; }
	public GatherResult TryGather(InventoryState inventory, InventoryItemDefinition dropItem, InventoryState toolInventory = null) { if (inventory == null || dropItem == null || IsDepleted) return GatherResult.Fail(); if (RequiresTool && (toolInventory == null || toolInventory.GetTotalAmount(RequiredToolItemId) <= 0)) return GatherResult.Fail("tool_required"); int added = inventory.TryAddItem(dropItem, DropAmountPerGather); if (added <= 0) return GatherResult.Fail("inventory_full"); ChargesLeft--; return GatherResult.Success(dropItem.Id, added, IsDepleted); }
}
