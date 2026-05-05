public sealed class BaseEnergyState
{
	public int Produced { get; private set; }
	public int Used { get; private set; }
	public int Free => System.Math.Max(0, Produced - Used);
	public bool IsOverloaded => Used > Produced;
	public BaseEnergyState(int produced = 0, int used = 0) { Produced = System.Math.Max(0, produced); Used = System.Math.Max(0, used); }
	public void AddProduction(int amount) { Produced += System.Math.Max(0, amount); }
	public bool TryReserve(int amount) { amount = System.Math.Max(0, amount); if (amount > Free) return false; Used += amount; return true; }
	public void Release(int amount) { Used = System.Math.Max(0, Used - System.Math.Max(0, amount)); }
}
