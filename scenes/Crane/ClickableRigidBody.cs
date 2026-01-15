using Godot;
using System;

[GlobalClass]
public partial class ClickableRigidBody : RigidBody3D
{
	private bool isBeingGrabbed = false;
    Vector3 grabOffset;

    public override void _PhysicsProcess(double delta)
    {
        if (!isBeingGrabbed)
		{
			return;
		}

        Vector3 target = GetMouseWorldPoint() + grabOffset;
        Vector3 error = target - GlobalPosition;

        Vector3 force = error * 90f - LinearVelocity * 14f;
        ApplyCentralForce(force * Mass);
    }

    Vector3 GetMouseWorldPoint()
    {
        var cam = GetViewport().GetCamera3D();
        var m = GetViewport().GetMousePosition();

        var from = cam.ProjectRayOrigin(m);
        var dir = cam.ProjectRayNormal(m);

        var q = PhysicsRayQueryParameters3D.Create(from, from + dir * 1000);
        var hit = GetWorld3D().DirectSpaceState.IntersectRay(q);

        return hit.Count > 0 ? (Vector3)hit["position"] : GlobalPosition;
    }

    public void HandleInput(Camera3D camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
	{
		if (Input.IsActionPressed("AddImpulse"))
		{
			Vector3 dir = -normal;
			float strength = 6f * Mass;
			
            ApplyImpulse(dir * strength, position - GlobalPosition);
            GetViewport().SetInputAsHandled();
        }

		if (!isBeingGrabbed && Input.IsActionPressed("DragObject"))
		{
			isBeingGrabbed = true;
            grabOffset = GlobalPosition - position;
		}

		if (Input.IsActionJustReleased("DragObject"))
		{
			isBeingGrabbed = false;
        }
    }
}
