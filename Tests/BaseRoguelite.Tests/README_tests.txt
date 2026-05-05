Запуск тестов:

  dotnet test Tests/BaseRoguelite.Tests/BaseRoguelite.Tests.csproj

Что проверяется сейчас:
- чистая логика инвентаря без Godot runtime
- лимит слотов
- лимит веса
- стакование
- частичное добавление
- коэффициент заполнения переносимого веса

Почему тестируется не PlayerInventoryState напрямую:
PlayerInventoryState наследуется от Godot.RefCounted и требует Godot runtime.
Для обычного xUnit-теста через dotnet test вынесена чистая логика в Core/InventoryLogic.
