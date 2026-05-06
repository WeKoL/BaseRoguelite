using System.Collections.Generic;
using System.Text;

public static class ControlHintBuilder020
{
	public static string Build(bool isInsideBase, bool hasInteractable, bool hasConsumables)
	{
		var parts = new List<string> { "WASD — движение", "Shift — бег", "Tab — меню", "ЛКМ — удар" };
		parts.Add(hasInteractable ? "E удерживать — взаимодействие" : "E — подобрать рядом");
		if (hasConsumables) parts.Add("H — быстро использовать аптечку/еду/воду");
		if (isInsideBase) parts.Add("База: доступны хранилище, крафт, лечение и улучшения");
		return string.Join(" | ", parts);
	}
}
