using Godot;

namespace PhysicsSimulationGodot.scripts;

[Tool]
public partial class SpinningCube : Node3D
{
    private RigidBody3D _cube;
    private MeshInstance3D _diagonalMeshInstance;
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
        _diagonalMeshInstance = GetNode<MeshInstance3D>("Cube/Diagonal/MeshInstance3D");

        ApplyTransform();

        if (!Engine.IsEditorHint())
        {
            ApplySpin();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        DrawDiagonal();
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

    private void DrawDiagonal()
    {
        if (_diagonalMeshInstance == null) return;

        Vector3 p1 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p2 = new Vector3(0.5f, 0.5f, 0.5f);

        Vector3 start =  p1;
        Vector3 end =  p2;

        ArrayMesh mesh = _diagonalMeshInstance.Mesh as ArrayMesh;
        if (mesh == null)
        {
            mesh = new ArrayMesh();
            _diagonalMeshInstance.Mesh = mesh;
        }

        var surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = new Vector3[] { start, end };

        mesh.ClearSurfaces();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, surfaceArray);
    }
}