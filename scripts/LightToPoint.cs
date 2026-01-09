using Godot;

namespace PhysicsSimulationGodot.scripts;

[Tool]
public partial class LightToPoint : DirectionalLight3D
{
	[Export]
	public Vector3 Target = Vector3.Zero;

	public override void _Process(double delta)
	{
		LookAt(Target, Vector3.Up);
	}
}