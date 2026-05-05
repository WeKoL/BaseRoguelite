using Godot;
public partial class DangerZoneArea : Area2D
{
	[Export] public WorldZoneKind ZoneKind{get;set;}=WorldZoneKind.Near; [Export] public int DangerLevel{get;set;}=1; [Export] public string DisplayName{get;set;}="Зона";
	public override void _Ready(){BodyEntered+=OnBodyEntered; BodyExited+=OnBodyExited;}
	private void OnBodyEntered(Node2D body){ if(body is PlayerController p) p.SetCurrentZone(new WorldZoneState(ZoneKind,DangerLevel,DisplayName)); }
	private void OnBodyExited(Node2D body){ if(body is PlayerController p) p.SetCurrentZone(null); }
}
