using Godot;
using System;

[GlobalClass]
public partial class ClickableRigidBody : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void HandleInput(Camera3D camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeIndex)
	{
		if(Input.IsActionPressed("AddImpulse"))
		{
			ApplyImpulse((eventPosition - camera.Position).Normalized(), eventPosition);
		}
	}
}
