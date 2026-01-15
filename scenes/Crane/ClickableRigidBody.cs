using Godot;
using System;

[GlobalClass]
public partial class ClickableRigidBody : RigidBody3D
{
    // No need to export those three, but wanted to avoid magic numbers
    [Export] 
    public float SpringStiffness = 50f;
    [Export] 
    public float SpringDamping = 5f;
    [Export] 
    public float MaxForce = 1000f;

    [Export] 
    public Node3D PinJointNode;

    private bool _isBeingGrabbed = false;
    private Vector3 _grabLocalOffset;
    private float _dragHeightY;

    public override void _Ready()
    {
        AxisLockLinearY = true;
        AxisLockAngularX = true;
        AxisLockAngularZ = true;

        AngularDamp = 2.0f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isBeingGrabbed) return;

        Vector3? mousePos = GetMouseOnPlane(_dragHeightY);
        if (mousePos.HasValue)
        {
            ApplySpringForce(mousePos.Value);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isBeingGrabbed && @event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
            {
                _isBeingGrabbed = false;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public void HandleInput(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
    {
        if (!_isBeingGrabbed && @event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.ButtonIndex == MouseButton.Left && mouseBtn.Pressed)
            {
                _isBeingGrabbed = true;
                _grabLocalOffset = ToLocal(position);
                _dragHeightY = position.Y;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private void ApplySpringForce(Vector3 targetGlobalPos)
    {
        Vector3 currentGrabGlobalPos = ToGlobal(_grabLocalOffset);
        Vector3 displacement = targetGlobalPos - currentGrabGlobalPos;
        Vector3 pointVelocity = LinearVelocity + AngularVelocity.Cross(currentGrabGlobalPos - GlobalPosition);

        Vector3 force = (displacement * SpringStiffness) - (pointVelocity * SpringDamping);

        if (PinJointNode != null)
        {
            float distToJoint = currentGrabGlobalPos.DistanceTo(PinJointNode.GlobalPosition);
            force *= (1.0f + distToJoint * 0.2f);
        }

        if (force.Length() > MaxForce)
        {
            force = force.Normalized() * MaxForce;
        }

        ApplyForce(force, currentGrabGlobalPos - GlobalPosition);
    }

    private Vector3? GetMouseOnPlane(float height)
    {
        var camera = GetViewport().GetCamera3D();
        var mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayNormal = camera.ProjectRayNormal(mousePos);
        Plane plane = new Plane(Vector3.Up, new Vector3(0, height, 0));

        return plane.IntersectsRay(rayOrigin, rayNormal);
    }
}