using Godot;
using System;
using System.Collections.Generic;

public partial class SpinnerUI : CanvasLayer
{
    public Button StartStopButton { get; private set; }
    public Button ResetButton { get; private set; }
    public Button ApplyButton { get; private set; }

    public HSlider EdgeLengthSlider { get; private set; }
    public HSlider DensitySlider { get; private set; }
    public HSlider AngularVelocitySlider { get; private set; }
    public HSlider InitialTiltSlider { get; private set; }
    public HSlider TraceSizeSlider { get; private set; }
    public HSlider TimeStepSlider { get; private set; }

    private HashSet<Control> alwaysEnabled;

    public override void _Ready()
    {
        base._Ready();

        StartStopButton = GetNode<Button>("VBoxContainer/StartStopButton");
        ResetButton = GetNode<Button>("VBoxContainer/ResetApplyRow/ResetButton");
        ApplyButton = GetNode<Button>("VBoxContainer/ResetApplyRow/ApplyButton");

        EdgeLengthSlider = GetNode<HSlider>("VBoxContainer/EdgeLengthRow/HSlider");
        DensitySlider = GetNode<HSlider>("VBoxContainer/DensityRow/HSlider");
        AngularVelocitySlider = GetNode<HSlider>("VBoxContainer/AngularVelocityRow/HSlider");
        InitialTiltSlider = GetNode<HSlider>("VBoxContainer/InitialTiltRow/HSlider");
        TraceSizeSlider = GetNode<HSlider>("VBoxContainer/TraceSizeRow/HSlider");
        TimeStepSlider = GetNode<HSlider>("VBoxContainer/TimestepRow/HSlider");

        alwaysEnabled = new HashSet<Control> { StartStopButton };
    }

    public void ToggleControls(bool enabled)
    {
        VBoxContainer boxContainer = GetNode<VBoxContainer>("VBoxContainer");
        SetBoxEnabled(boxContainer, enabled);
    }

    private void SetBoxEnabled(BoxContainer boxContainer, bool enabled)
    {
        SetMouseRecursive(boxContainer, enabled);
        SetControlsRecursive(boxContainer, enabled, alwaysEnabled);
        boxContainer.Modulate = enabled
            ? Colors.White
            : new Color(1f, 1f, 1f, 0.5f);
    }

    private void SetMouseRecursive(Control control, bool enabled)
    {
        control.MouseFilter = enabled
            ? Control.MouseFilterEnum.Stop
            : Control.MouseFilterEnum.Ignore;

        foreach (Node child in control.GetChildren())
        {
            if (child is Control childControl)
            {
                SetMouseRecursive(childControl, enabled);
            }
        }
    }

    private void SetControlsRecursive(Control control, bool enabled, HashSet<Control> skipControls)
    {
        foreach (Node child in control.GetChildren())
        {
            if (child is Control childControl)
            {
                if (!skipControls.Contains(childControl))
                {
                    if (childControl is BaseButton button)
                        button.Disabled = !enabled;
                    else if (childControl is Slider slider)
                        slider.Editable = enabled;
                }

                SetControlsRecursive(childControl, enabled, skipControls);
            }
        }
    }
}
