@'
# BaseRoguelite — developer notes

Проект: BaseRoguelite  
Движок: Godot .NET / C#  
Основная ветка: main  
Рабочие ветки: feature/0.2.n-name

## Структура проекта

Core/       - чистая игровая логика без привязки к сценам Godot
Features/   - игровые сцены, UI, игрок, база, враги и привязка логики к Godot
Main/       - главная сцена и точка запуска
Shared/     - общие интерфейсы, UI-хелперы, feedback и interaction
Content/    - игровые ресурсы, предметы, иконки
Tests/      - тесты логики
Docs/       - документация проекта

## Правила разработки

Перед началом работы:

```powershell
git checkout main
git pull
git checkout -b feature/0.2.x-short-name