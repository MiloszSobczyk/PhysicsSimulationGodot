using Godot;

public partial class FlyCamera : Camera3D
{
    [Export] public float BaseSpeed = 6.0f;
    [Export] public float BoostMultiplier = 4.0f;
    [Export] public float SlowMultiplier = 0.25f;
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float Acceleration = 10.0f;

    private Vector3 _velocity = Vector3.Zero;
    private float _pitch = 0.0f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (Input.IsActionPressed("DragCamera"))
            {
                _pitch -= mouseMotion.Relative.Y * MouseSensitivity;
                _pitch = Mathf.Clamp(
                    _pitch,
                    -Mathf.Pi / 2.0f + 0.1f,
                    Mathf.Pi / 2.0f - 0.1f
                );

                RotateY(-mouseMotion.Relative.X * MouseSensitivity);

                Rotation = new Vector3(
                    _pitch,
                    Rotation.Y,
                    0.0f
                );
            }
        }
    }


    public override void _Process(double delta)
    {
        Vector3 inputDir = Vector3.Zero;

        if (Input.IsActionPressed("MoveForward"))
        {
            inputDir -= Transform.Basis.Z;
        }
        if (Input.IsActionPressed("MoveBackward"))
        {
            inputDir += Transform.Basis.Z;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            inputDir -= Transform.Basis.X;
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            inputDir += Transform.Basis.X;
        }
        if (Input.IsActionPressed("MoveUp"))
        {
            inputDir += Transform.Basis.Y;
        }
        if (Input.IsActionPressed("MoveDown"))
        {
            inputDir -= Transform.Basis.Y;
        }

        if (inputDir != Vector3.Zero)
        {
            inputDir = inputDir.Normalized();
        }

        float speed = BaseSpeed;

        _velocity = _velocity.Lerp(inputDir * speed, Acceleration * (float)delta);
        Position += _velocity * (float)delta;
    }
}
