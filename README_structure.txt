Новая структура проекта

Main/ - точка входа сцены и логики запуска
Core/Items/ - общие классы предметной системы
Features/Player/ - сцена игрока и логика игрока
Features/Inventory/ - инвентарь игрока и его UI
Features/Base/ - база, склад и связанные UI-сцены
Features/Menu/ - общее меню и навигация по меню
Shared/UI/ - переиспользуемые UI-элементы
Shared/Interaction/ - общая логика взаимодействия с объектами мира

Перенос сделан без перехода на .tres. Изменены только папки и ссылки на пути в сценах/коде.

Старый путь -> новый путь
Scenes/Base/BaseRoot.tscn -> Features/Base/BaseRoot.tscn
Scenes/Main/Main.tscn -> Main/Main.tscn
Scenes/Player/Player.tscn -> Features/Player/Player.tscn
Scenes/UI/GameMenu.tscn -> Features/Menu/GameMenu.tscn
Scenes/UI/InventoryPanel.tscn -> Features/Inventory/InventoryPanel.tscn
Scenes/UI/InventorySlotCard.tscn -> Features/Inventory/InventorySlotCard.tscn
Scenes/UI/ItemCard.tscn -> Shared/UI/ItemCard.tscn
Scenes/UI/ItemCardDemo.tscn -> Shared/UI/ItemCardDemo.tscn
Scenes/UI/StorageCategorySection.tscn -> Features/Base/StorageCategorySection.tscn
Scenes/UI/StorageSubpanel.tscn -> Features/Base/StorageSubpanel.tscn
Scripts/Base/BaseRoot.cs -> Features/Base/BaseRoot.cs
Scripts/Base/BaseStorageState.cs -> Features/Base/BaseStorageState.cs
Scripts/Interaction/IMenuInteractable.cs -> Shared/Interaction/IMenuInteractable.cs
Scripts/Interaction/MenuContextInteractable.cs -> Shared/Interaction/MenuContextInteractable.cs
Scripts/Items/InventorySlot.cs -> Features/Inventory/InventorySlot.cs
Scripts/Items/ItemCategory.cs -> Core/Items/ItemCategory.cs
Scripts/Items/ItemData.cs -> Core/Items/ItemData.cs
Scripts/Items/ItemEntry.cs -> Core/Items/ItemEntry.cs
Scripts/Items/ItemRarity.cs -> Core/Items/ItemRarity.cs
Scripts/Main/Main.cs -> Main/Main.cs
Scripts/Player/InteractionProgressRing.cs -> Features/Player/InteractionProgressRing.cs
Scripts/Player/PlayerController.cs -> Features/Player/PlayerController.cs
Scripts/Player/PlayerInventoryState.cs -> Features/Inventory/PlayerInventoryState.cs
Scripts/UI/BaseActionId.cs -> Features/Menu/BaseActionId.cs
Scripts/UI/CrosshairCursor.cs -> Features/Menu/CrosshairCursor.cs
Scripts/UI/GameMenu.cs -> Features/Menu/GameMenu.cs
Scripts/UI/GameMenuContext.cs -> Features/Menu/GameMenuContext.cs
Scripts/UI/InventorySlotCard.cs -> Features/Inventory/InventorySlotCard.cs
Scripts/UI/ItemCard.cs -> Shared/UI/ItemCard.cs
Scripts/UI/ItemCardDemo.cs -> Shared/UI/ItemCardDemo.cs
Scripts/UI/MenuSection.cs -> Features/Menu/MenuSection.cs
Scripts/UI/StorageCategorySection.cs -> Features/Base/StorageCategorySection.cs
Scripts/UI/StorageSubpanel.cs -> Features/Base/StorageSubpanel.cs
