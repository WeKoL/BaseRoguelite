public static class ConfirmationPromptLogic
{
	public static bool RequiresConfirmation(DangerousActionKind kind, ItemRarity rarity = ItemRarity.Common, int amount = 1) { if (kind == DangerousActionKind.DeleteSave || kind == DangerousActionKind.LeaveExpedition) return true; if (kind == DangerousActionKind.DropItem && (rarity >= ItemRarity.Rare || amount >= 10)) return true; if (kind == DangerousActionKind.CancelCraft && rarity >= ItemRarity.Epic) return true; return false; }
	public static string BuildMessage(DangerousActionKind kind, string subject) { subject = string.IsNullOrWhiteSpace(subject) ? "действие" : subject; return kind switch { DangerousActionKind.DropItem => $"Точно выбросить {subject}?", DangerousActionKind.DeleteSave => $"Точно удалить сохранение {subject}?", DangerousActionKind.CancelCraft => $"Точно отменить крафт {subject}?", DangerousActionKind.LeaveExpedition => $"Точно завершить экспедицию {subject}?", _ => $"Подтвердить {subject}?" }; }
}
