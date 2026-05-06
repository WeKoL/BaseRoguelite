using System;
using System.Collections.Generic;
using System.Linq;

public static class Version040Info
{
	public const string Version = "0.4.0";
	public const string SaveVersion = "6";
	public const string Goal = "Логическое ядро BaseRoguelite: единая симуляция инвентаря, базы, крафта, выживания, боя, мира, квестов и сохранений без обязательной привязки к Godot-сценам.";

	public static IReadOnlyList<CoreBranch040> Branches { get; } = new[]
	{
		new CoreBranch040(1, "Техническая стабилизация", "манифест систем, баланс-правила, quality gate"),
		new CoreBranch040(2, "Инвентарь и экипировка", "стаки, перенос, экипировка, быстрые слоты"),
		new CoreBranch040(3, "Хранилище базы", "лимиты, резервы, авторазгрузка, аудит"),
		new CoreBranch040(4, "Крафт и станки", "рецепты, уровни станков, партия крафта"),
		new CoreBranch040(5, "Здоровье и выживание", "HP, стамина, еда, вода, статусы"),
		new CoreBranch040(6, "Сбор ресурсов", "ресурсные узлы, инструменты, лут"),
		new CoreBranch040(7, "Враги и бой", "урон, броня, ИИ, волны"),
		new CoreBranch040(8, "Мир и зоны", "экспедиция, риск, события, эвакуация"),
		new CoreBranch040(9, "База", "улучшения, энергия, оборона, комфорт"),
		new CoreBranch040(10, "Сохранения", "снимок состояния, миграции, валидация"),
		new CoreBranch040(11, "Интерфейс", "view-model, подсказки, действия меню"),
		new CoreBranch040(12, "Feedback", "очередь событий, интенсивность, пакеты"),
		new CoreBranch040(13, "Квесты", "цели, прогресс, награды, обучение"),
		new CoreBranch040(14, "Тесты и качество", "контроль покрытия веток и релизные блокеры")
	};

	public static bool CoversEveryBranch() => Branches.Select(x => x.Number).OrderBy(x => x).SequenceEqual(Enumerable.Range(1, 14));
}

public sealed class CoreBranch040
{
	public int Number { get; }
	public string Title { get; }
	public string CoreFocus { get; }

	public CoreBranch040(int number, string title, string coreFocus)
	{
		Number = number;
		Title = title ?? string.Empty;
		CoreFocus = coreFocus ?? string.Empty;
	}
}

public sealed class CoreManifest040
{
	private readonly HashSet<int> _implementedBranches = new();
	private readonly Dictionary<int, int> _testCountByBranch = new();
	private readonly List<string> _notes = new();

	public IReadOnlyList<string> Notes => _notes;

	public void MarkImplemented(int branchNumber, int tests, string note)
	{
		if (branchNumber < 1 || branchNumber > 14)
			throw new ArgumentOutOfRangeException(nameof(branchNumber));

		_implementedBranches.Add(branchNumber);
		_testCountByBranch[branchNumber] = Math.Max(0, tests);
		if (!string.IsNullOrWhiteSpace(note))
			_notes.Add($"{branchNumber}: {note}");
	}

	public bool HasAllBranches => Enumerable.Range(1, 14).All(_implementedBranches.Contains);
	public int TotalDeclaredTests => _testCountByBranch.Values.Sum();
	public IReadOnlyList<int> MissingBranches => Enumerable.Range(1, 14).Where(x => !_implementedBranches.Contains(x)).ToArray();
	public bool HasMinimumTestsPerBranch(int minTests) => Enumerable.Range(1, 14).All(x => _testCountByBranch.TryGetValue(x, out int count) && count >= minTests);
}
