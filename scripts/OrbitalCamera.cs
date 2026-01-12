using Godot;

namespace PhysicsSimulationGodot.scripts;

[Tool]
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

    private float _yaw;
    private float _pitch;
    private float _distance;

    private readonly Vector3 _target = Vector3.Zero;

    public override void _Ready()
    {
        Vector3 offset = GlobalPosition - _target;
        _distance = offset.Length();

        _yaw = Mathf.Atan2(offset.X, offset.Z);
        _pitch = Mathf.Asin(offset.Y / _distance);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion &&
            Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _yaw -= motion.Relative.X * RotationSpeed;

            _pitch = Mathf.Clamp(
                _pitch + motion.Relative.Y * RotationSpeed,
                -1.3f,
                1.3f
            );
        }

        if (@event is InputEventMouseButton wheel &&
            wheel.Pressed)
        {
            if (wheel.ButtonIndex == MouseButton.WheelUp)
            {
                _distance -= ZoomSpeed;
            }
            else if (wheel.ButtonIndex == MouseButton.WheelDown)
            {
                _distance += ZoomSpeed;
            }

            _distance = Mathf.Clamp(
                _distance,
                MinDistance,
                MaxDistance
            );
        }
    }

    public override void _Process(double delta)
    {
        Vector3 position = new Vector3(
            _distance * Mathf.Cos(_pitch) * Mathf.Sin(_yaw),
            _distance * Mathf.Sin(_pitch),
            _distance * Mathf.Cos(_pitch) * Mathf.Cos(_yaw)
        );

        GlobalPosition = _target + position;
        LookAt(_target, Vector3.Up);
    }
}
