using Godot;
using PhysicsSimulationGodot.scripts.structures;
using System.Collections.Generic;

namespace PhysicsSimulationGodot.scripts;

[Tool]
public partial class Spinner : Node3D
{
    private RigidBody3D _cube;
    private MeshInstance3D _diagonalMeshInstance;
    private MeshInstance3D _traceMeshInstance;
    private readonly Queue<Vector3> _tracePoints = new();
    private SpinnerParameters _params = new();
    private SpinnerUI _ui;

    public bool Running { get; private set; } = true;

    public override void _Ready()
    {
        _cube = GetNode<RigidBody3D>("Cube");

        _diagonalMeshInstance = GetNode<MeshInstance3D>("Cube/Diagonal/MeshInstance3D");
        _traceMeshInstance = GetNode<MeshInstance3D>("Trace/MeshInstance3D");
        GD.Print("Dupa");

        SetupUI();

        ApplyTransform();

        if (!Engine.IsEditorHint())
        {
            ApplySpin();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        UpdateDiagonal();

        if (!Engine.IsEditorHint())
        {
            UpdateTrace();
        }
    }
    private void Stop()
    {
        if (!Running) return;

        _params.SavedAngularVelocity = _cube.AngularVelocity;
        _params.SavedLinearVelocity = _cube.LinearVelocity;

        _cube.Freeze = true;

        Running = false;
    }
    private void Start()
    {
        if (Running) return;

        _cube.Freeze = false;

        _cube.AngularVelocity = _params.SavedAngularVelocity;
        _cube.LinearVelocity = _params.SavedLinearVelocity;

        Running = true;
    }
    private void ToggleCube()
    {
        if (Running)
        {
            _ui.ToggleControls(true);
            Stop();
        }
        else
        {
            _ui.ToggleControls(false);
            Start();
        }
    }
    private void SetupUI()
    {
        _ui = GetNode<SpinnerUI>("SpinnerUI");
        _ui.StartStopButton.SetPressedNoSignal(Running);
        _ui.StartStopButton.Pressed += () =>
        {
            ToggleCube();
        };

        _ui.EdgeLengthSlider.SetValueNoSignal(_params.EdgeLength);
        _ui.EdgeLengthSlider.ValueChanged += (value) =>
        {
            _params.EdgeLength = value;
        };

        _ui.DensitySlider.SetValueNoSignal(_params.Density);
        _ui.DensitySlider.ValueChanged += (value) =>
        {
            _params.Density = value;
        };

        _ui.AngularVelocitySlider.SetValueNoSignal(_params.SpinSpeed);
        _ui.AngularVelocitySlider.ValueChanged += (value) =>
        {
            _params.SpinSpeed = value;
        };

        _ui.InitialTiltSlider.SetValueNoSignal(_params.InitialTilt);
        _ui.InitialTiltSlider.ValueChanged += (value) =>
        {
            _params.InitialTilt = value;
        };

        _ui.TraceSizeSlider.SetValueNoSignal(_params.TraceSize);
        _ui.TraceSizeSlider.ValueChanged += (value) =>
        {
            _params.TraceSize = (int)value;
        };

        _ui.TimeStepSlider.SetValueNoSignal(_params.TimeStep);
        _ui.TimeStepSlider.ValueChanged += (value) =>
        {
            _params.TimeStep = value;
        };
    }

    private void ApplySpin()
    {
        if (_cube == null) return;

        _cube.AngularDamp = 0.0f;
        _cube.LinearDamp = 0.0f;

        Vector3 localSpinAxis = new Vector3(1.0f, 1.0f, 1.0f).Normalized();
        Vector3 globalSpinAxis = _cube.GlobalBasis * localSpinAxis;

        _cube.AngularVelocity = globalSpinAxis * (float)_params.SpinSpeed;
    }
    private void ApplyTransform()
    {
        if (_cube == null) return;

        Vector3 localDiagonal = new Vector3(1.0f, 1.0f, 1.0f).Normalized();
        Quaternion alignQuat = new Quaternion(localDiagonal, Vector3.Up);

        double tiltRadians = Mathf.DegToRad(_params.InitialTilt);
        Quaternion tiltQuat = new Quaternion(Vector3.Right, (float)tiltRadians);

        Quaternion finalRotation = tiltQuat * alignQuat;
        _cube.Quaternion = finalRotation;

        Vector3 bottomCorner = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 currentCornerOffset = finalRotation * bottomCorner;
        _cube.Position = -currentCornerOffset;
    }
    private void UpdateTrace()
    {
        if (_traceMeshInstance == null || _cube == null) return;

        Vector3 localTip = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 globalTipPos = _cube.ToGlobal(localTip);

        Vector3 pointForTrace = ToLocal(globalTipPos);

        _tracePoints.Enqueue(pointForTrace);

        if (_tracePoints.Count > _params.TraceSize)
        {
            _tracePoints.Dequeue();
        }

        if (_tracePoints.Count < 2) return;

        ArrayMesh mesh = _traceMeshInstance.Mesh as ArrayMesh;
        if (mesh == null)
        {
            mesh = new ArrayMesh();
            _traceMeshInstance.Mesh = mesh;
        }

        var pointsArray = _tracePoints.ToArray();

        var surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);
        surfaceArray[(int)Mesh.ArrayType.Vertex] = pointsArray;

        mesh.ClearSurfaces();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.LineStrip, surfaceArray);
    }
    private void UpdateDiagonal()
    {
        if (_diagonalMeshInstance == null) return;

        Vector3 start = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 end = new Vector3(0.5f, 0.5f, 0.5f);

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