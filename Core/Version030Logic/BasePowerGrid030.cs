using System.Collections.Generic;

public sealed class BasePowerGrid030
{
	private readonly Dictionary<string, int> _consumers = new();
	public int GeneratedPower { get; private set; }
	public IReadOnlyDictionary<string, int> Consumers => _consumers;
	public int UsedPower { get { int sum = 0; foreach (int value in _consumers.Values) sum += value; return sum; } }
	public int FreePower => System.Math.Max(0, GeneratedPower - UsedPower);
	public bool IsOverloaded => UsedPower > GeneratedPower;
	public BasePowerGrid030(int generatedPower) { GeneratedPower = System.Math.Max(0, generatedPower); }
	public void SetGeneratedPower(int power) => GeneratedPower = System.Math.Max(0, power);
	public bool AddConsumer(string id, int power)
	{
		if (string.IsNullOrWhiteSpace(id) || power < 0) return false;
		_consumers[id] = power;
		return true;
	}
}
