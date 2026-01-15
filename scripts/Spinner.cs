using Godot;
using PhysicsSimulationGodot.scripts.structures;
using System;
using System.Collections.Generic;

namespace PhysicsSimulationGodot.scripts;

[Tool]
public partial class Spinner : Node3D
{
    private RigidBody3D _cube;
    private MeshInstance3D _diagonalMeshInstance;
    private MeshInstance3D _traceMeshInstance;
    private SpinnerUI _ui;

    private readonly Queue<Vector3> _tracePoints = new();
    private SpinnerParameters _params = new();

    public bool Running { get; private set; } = true;

    public override void _Ready()
    {
        _cube = GetNode<RigidBody3D>("Cube");

        _diagonalMeshInstance = GetNode<MeshInstance3D>("Cube/Diagonal/MeshInstance3D");
        _traceMeshInstance = GetNode<MeshInstance3D>("Trace/MeshInstance3D");

        SetupUI();
        ApplyParameters();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!Running) return;

        UpdateDiagonal();

        if (!Engine.IsEditorHint())
        {
            UpdateTrace();
        }
    }
    private void ApplyTimeStep()
    {
        if (_params.TimeStep <= 0.0)
        {
            return;
        }

        int ticksPerSecond = Mathf.RoundToInt((float)(1.0 / _params.TimeStep));
        ticksPerSecond = Mathf.Clamp(ticksPerSecond, 1, 1000);

        Engine.PhysicsTicksPerSecond = ticksPerSecond;
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

    private void Reset()
    {
        _params.ResetToDefaults();

        _ui.EdgeLengthSlider.SetValueNoSignal(_params.EdgeLength);
        _ui.DensitySlider.SetValueNoSignal(_params.Density);
        _ui.AngularVelocitySlider.SetValueNoSignal(_params.SpinSpeed);
        _ui.InitialTiltSlider.SetValueNoSignal(_params.InitialTilt);
        _ui.TraceSizeSlider.SetValueNoSignal(_params.TraceSize);
        _ui.TimeStepSlider.SetValueNoSignal(_params.TimeStep);

        ApplyParameters();
    }

    private void SetupUI()
    {
        _ui = GetNode<SpinnerUI>("SpinnerUI");
        _ui.ToggleControls(false);

        _ui.StartStopButton.SetPressedNoSignal(Running);
        _ui.StartStopButton.Pressed += () =>
        {
            ToggleCube();
        };

        _ui.ResetButton.Pressed += () =>
        {
            Reset();
            ApplyParameters();
        };

        _ui.ApplyButton.Pressed += () =>
        {
            ApplyParameters();
        };

        BindSliderAndSpinBox(
            _ui.EdgeLengthSlider,
            _ui.EdgeLengthSpinBox,
            _params.EdgeLength,
            value =>
            {
                _params.EdgeLength = value;
            }
        );

        BindSliderAndSpinBox(
            _ui.DensitySlider,
            _ui.DensitySpinBox,
            _params.Density,
            value =>
            {
                _params.Density = value;
            }
        );

        BindSliderAndSpinBox(
            _ui.AngularVelocitySlider,
            _ui.AngularVelocitySpinBox,
            _params.SpinSpeed,
            value =>
            {
                _params.SpinSpeed = value;
            }
        );

        BindSliderAndSpinBox(
            _ui.InitialTiltSlider,
            _ui.InitialTiltSpinBox,
            _params.InitialTilt,
            value =>
            {
                _params.InitialTilt = value;
            }
        );

        BindSliderAndSpinBox(
            _ui.TraceSizeSlider,
            _ui.TraceSizeSpinBox,
            _params.TraceSize,
            value =>
            {
                _params.TraceSize = (int)value;
            }
        );

        BindSliderAndSpinBox(
            _ui.TimeStepSlider,
            _ui.TimeStepSpinBox,
            _params.TimeStep,
            value =>
            {
                _params.TimeStep = value;
            }
        );
    }
    private void BindSliderAndSpinBox(Slider slider, SpinBox spinBox, double initialValue, Action<double> setter)
    {
        slider.SetValueNoSignal(initialValue);
        spinBox.SetValueNoSignal(initialValue);

        slider.ValueChanged += (value) =>
        {
            setter(value);
            spinBox.SetValueNoSignal(value);
        };

        spinBox.ValueChanged += (value) =>
        {
            setter(value);
            slider.SetValueNoSignal(value);
        };
    }

    private void ApplyParameters()
    {
        _tracePoints.Clear();

        ApplyTimeStep();

        _cube.Mass = (float)(_params.Density * System.Math.Pow(_params.EdgeLength, 3));
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

        Vector3 newAngularVelocity = globalSpinAxis * (float)_params.SpinSpeed;

        _cube.AngularVelocity = newAngularVelocity;
        _params.SavedAngularVelocity = newAngularVelocity;

        _params.SavedLinearVelocity = Vector3.Zero;
        _cube.LinearVelocity = Vector3.Zero;
    }

    private void ApplyTransform()
    {
        if (_cube == null) return;

        Vector3 halfEdge = Vector3.One * ((float)_params.EdgeLength * 0.5f);
        Quaternion alignQuat = new Quaternion(halfEdge.Normalized(), Vector3.Up);

        double tiltRadians = Mathf.DegToRad(_params.InitialTilt);
        Quaternion tiltQuat = new Quaternion(Vector3.Right, (float)tiltRadians);

        Quaternion finalRotation = tiltQuat * alignQuat;
        _cube.Quaternion = finalRotation;

        Vector3 bottomCorner = -halfEdge;
        Vector3 currentCornerOffset = finalRotation * bottomCorner;
        _cube.Position = -currentCornerOffset;

        _cube.Scale = new Vector3((float)_params.EdgeLength, (float)_params.EdgeLength, (float)_params.EdgeLength);
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
