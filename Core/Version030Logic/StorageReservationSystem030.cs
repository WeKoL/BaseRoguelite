using System.Collections.Generic;

public sealed class StorageReservationSystem030
{
	private readonly Dictionary<string, int> _reserved = new();
	public IReadOnlyDictionary<string, int> Reserved => _reserved;

	public bool Reserve(string itemId, int amount)
	{
		if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;
		_reserved[itemId] = GetReserved(itemId) + amount;
		return true;
	}

	public int GetReserved(string itemId)
	{
		return !string.IsNullOrWhiteSpace(itemId) && _reserved.TryGetValue(itemId, out int amount) ? amount : 0;
	}

	public int GetAvailable(StorageState storage, string itemId)
	{
		if (storage == null) return 0;
		return System.Math.Max(0, storage.GetTotalAmount(itemId) - GetReserved(itemId));
	}

	public void Clear() => _reserved.Clear();
}
