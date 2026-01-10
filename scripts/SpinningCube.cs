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

    public override void _Ready()
    {
        _cube = GetNode<RigidBody3D>("Cube");
        ApplyTransform();
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