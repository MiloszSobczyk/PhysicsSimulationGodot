using Godot;

namespace PhysicsSimulationGodot.scripts;

public partial class Simulation : Node3D
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}