# BaseRoguelite 0.3.1-dev branch expansion

Этот слой не меняет официальный тег `0.3.0`. Он добавляет рабочие заготовки и тесты для дальнейшего развития всех 14 веток.

## Что добавлено

- `Core/Version031Logic/Branch01TechnicalRoadmap031.cs` — backlog, checklist, release gate context.
- `Core/Version031Logic/Branch02InventoryEquipment031.cs` — план действий предметов, сравнение экипировки.
- `Core/Version031Logic/Branch03Storage031.cs` — политики хранилища и аудит стаков.
- `Core/Version031Logic/Branch04Crafting031.cs` — preview крафта и зависимости рецептов.
- `Core/Version031Logic/Branch05Survival031.cs` — отдых, холод, радиация.
- `Core/Version031Logic/Branch06Gathering031.cs` — маршрут добычи и баланс респавна.
- `Core/Version031Logic/Branch07Combat031.cs` — оценка угроз, тактика боя, запас патронов.
- `Core/Version031Logic/Branch08World031.cs` — открытие зон и расписание событий мира.
- `Core/Version031Logic/Branch09Base031.cs` — зависимости построек, очередь проектов, комфорт базы.
- `Core/Version031Logic/Branch10Save031.cs` — профиль сохранения, autosave, integrity report.
- `Core/Version031Logic/Branch11Ui031.cs` — стек меню и приоритет подсказок.
- `Core/Version031Logic/Branch12Feedback031.cs` — профиль обратной связи и batch событий.
- `Core/Version031Logic/Branch13Quest031.cs` — milestones квестов и обучающий flow.
- `Core/Version031Logic/Branch14QualityGate031.cs` — план покрытия тестами и общий quality gate.
- `Tests/BaseRoguelite.Tests/Version031/Version031BranchExpansionTests.cs` — тесты для всех 14 веток.

## Проверка

В среде с .NET SDK:

```powershell
dotnet test Tests/BaseRoguelite.Tests/BaseRoguelite.Tests.csproj
```

В Godot .NET: открыть `project.godot`, нажать `Build`, затем запуск сцены `Main/Main.tscn`.

## Важно

Этот слой специально сделан в чистой логике. Он не должен ломать текущие сцены. Подключение в UI/мир лучше делать маленькими шагами: сначала инвентарь/экипировка, затем хранилище/крафт, затем бой/мир/квесты.
