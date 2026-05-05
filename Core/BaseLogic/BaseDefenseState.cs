public sealed class BaseDefenseState
{
	public int BarrierStrength { get; private set; }
	public int TurretPower { get; private set; }
	public int ThreatPressure { get; private set; }
	public bool IsBreached => ThreatPressure > BarrierStrength + TurretPower;
	public BaseDefenseState(int barrierStrength, int turretPower = 0) { BarrierStrength = System.Math.Max(0, barrierStrength); TurretPower = System.Math.Max(0, turretPower); }
	public void ApplyThreat(int amount) { ThreatPressure += System.Math.Max(0, amount); }
	public int Repair(int amount) { int before = BarrierStrength; BarrierStrength += System.Math.Max(0, amount); return BarrierStrength - before; }
}
