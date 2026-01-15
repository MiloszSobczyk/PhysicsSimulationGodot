using Godot;

[GlobalClass]
public partial class OrbitalCamera : Camera3D
{
    [Export]
    public float RotationSpeed = 0.01f;

    [Export]
    public float ZoomSpeed = 1.0f;

    [Export]
    public float MinDistance = 2.0f;

    [Export]
    public float MaxDistance = 15.0f;

    [Export]
    public Node3D Target;

    private float _currentRotationX = 0f;
    private float _currentRotationY = 0f;
    private float _currentDistance = 5f;

    public override void _Ready()
    {
        if (Target != null)
        {
            _currentDistance = GlobalPosition.DistanceTo(Target.GlobalPosition);
            LookAt(Target.GlobalPosition);
        }

        Vector3 currentRot = Rotation;
        _currentRotationX = currentRot.Y;
        _currentRotationY = currentRot.X;

        _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, MaxDistance);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (Input.IsActionPressed("DragCamera"))
            {
                _currentRotationX -= mouseMotion.Relative.X * RotationSpeed;
                _currentRotationY -= mouseMotion.Relative.Y * RotationSpeed;
                _currentRotationY = Mathf.Clamp(_currentRotationY, -Mathf.Pi / 2.0f + 0.1f, Mathf.Pi / 2.0f - 0.1f);
            }
        }

        if (@event.IsActionPressed("ZoomIn"))
        {
            _currentDistance -= ZoomSpeed;
            _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, MaxDistance);
        }
        else if (@event.IsActionPressed("ZoomOut"))
        {
            _currentDistance += ZoomSpeed;
            _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, MaxDistance);
        }
    }

    public override void _Process(double delta)
    {
        UpdateCameraTransform();
    }

    private void UpdateCameraTransform()
    {
        Vector3 targetPosition = Vector3.Zero;
        if (Target != null)
        {
            targetPosition = Target.GlobalPosition;
        }

        Quaternion rotation = Quaternion.FromEuler(new Vector3(_currentRotationY, _currentRotationX, 0));
        Vector3 offset = rotation * Vector3.Back * _currentDistance;
        GlobalPosition = targetPosition + offset;

        LookAt(targetPosition);
    }
}