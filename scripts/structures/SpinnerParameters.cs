namespace PhysicsSimulationGodot.scripts.structures;

public class SpinnerParameters
{
    public double EdgeLength { get; set; } = 1.0f;
    public double Density { get; set; } = 1.0f;
    public double SpinSpeed { get; set; } = 20.0f;
    public double InitialTilt { get; set; } = 0.0f;
    public int TraceSize { get; set; } = 1000;
    public double TimeStep { get; set; }
    public Godot.Vector3 SavedAngularVelocity { get; set; }
    public Godot.Vector3 SavedLinearVelocity { get; set; }

    public void ResetToDefaults()
    {
        EdgeLength = 1.0f;
        Density = 1.0f;
        SpinSpeed = 20.0f;
        InitialTilt = 0.0f;
        TraceSize = 1000;
        TimeStep = 0.01f;
    }
}
