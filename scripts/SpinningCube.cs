using Godot;

namespace PhysicsSimulationGodot.scripts;

[Tool]
public partial class SpinningCube : Node3D
{
    private RigidBody3D _cube;
    private float _tiltAngle = 0.0f;

    [Export(PropertyHint.Range, "0, 90")]
    public float TiltAngle
    {
        get => _tiltAngle;
        set
        {
            _tiltAngle = value;
            ApplyTransform();
        }
    }

    [Export]
    public float SpinSpeed = 20.0f;

    public override void _Ready()
    {
        _cube = GetNode<RigidBody3D>("Cube");
        
        ApplyTransform();

        if (!Engine.IsEditorHint())
        {
            ApplySpin();
        }
    }

    private void ApplySpin()
    {
        if (_cube == null) return;

        _cube.AngularDamp = 0.0f;
        _cube.LinearDamp = 0.0f;

        Vector3 localSpinAxis = new Vector3(1.0f, 1.0f, 1.0f).Normalized();

        Vector3 globalSpinAxis = _cube.GlobalBasis * localSpinAxis;

        _cube.AngularVelocity = globalSpinAxis * SpinSpeed;
    }

    private void ApplyTransform()
    {
        if (_cube == null) return;

        Vector3 localDiagonal = new Vector3(1.0f, 1.0f, 1.0f).Normalized();
        Quaternion alignQuat = new Quaternion(localDiagonal, Vector3.Up);

        float tiltRadians = Mathf.DegToRad(_tiltAngle);
        Quaternion tiltQuat = new Quaternion(Vector3.Right, tiltRadians);
        
        Quaternion finalRotation = tiltQuat * alignQuat;
        _cube.Quaternion = finalRotation;

        Vector3 bottomCorner = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 currentCornerOffset = finalRotation * bottomCorner;
        _cube.Position = -currentCornerOffset;
    }
}