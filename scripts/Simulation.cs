using Godot;
using System.Runtime.CompilerServices;

namespace PhysicsSimulationGodot.scripts;

public partial class Simulation : Node3D
{
    private Spinner _spinner;
    private Node3D _crane;
    private CheckButton _viewSwitcherButton;

    bool SpinnerVisible => _spinner.Visible;
    
    public override void _Ready()
	{
        _spinner = GetNode<Spinner>("Spinner");
        _crane = GetNode<Node3D>("Crane");
        _viewSwitcherButton = GetNode<CheckButton>("SimulationUI/ViewSwitcher");

        Input.MouseMode = Input.MouseModeEnum.Visible;

        _viewSwitcherButton.Pressed += () =>
        {
            ToggleView();
            _viewSwitcherButton.Text = SpinnerVisible ? "Spinner" : "Crane";
        };

        ShowSpinner();
    }

    private void ToggleView()
    {
        if (SpinnerVisible)
        {
            _spinner.StopFromOutisde();
            ShowCrane();
        }
        else
        {
            ShowSpinner();
        }
    }
    private void ShowSpinner()
    {
        _spinner.Visible = true;
        _spinner.SetProcess(true);
        _spinner.SetPhysicsProcess(true);

        _crane.Visible = false;
        _crane.SetProcess(false);
        _crane.SetPhysicsProcess(false);

        _spinner.GetNode<Control>("SpinnerUI").Visible = true;
    }
    private void ShowCrane()
    {
        _crane.Visible = true;
        _crane.SetProcess(true);
        _crane.SetPhysicsProcess(true);

        _spinner.Visible = false;
        _spinner.SetProcess(false);
        _spinner.SetPhysicsProcess(false);

        _spinner.GetNode<Control>("SpinnerUI").Visible = false;
    }

}