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

    public override void _Ready()
    {
        base._Ready();

        StartStopButton = GetNode<Button>("TopVBox/StartStopButton");
        ResetButton = GetNode<Button>("TopVBox/VBoxContainer/ResetApplyRow/ResetButton");
        ApplyButton = GetNode<Button>("TopVBox/VBoxContainer/ResetApplyRow/ApplyButton");

        EdgeLengthSlider = GetNode<HSlider>("TopVBox/VBoxContainer/EdgeLengthRow/HSlider");
        DensitySlider = GetNode<HSlider>("TopVBox/VBoxContainer/DensityRow/HSlider");
        AngularVelocitySlider = GetNode<HSlider>("TopVBox/VBoxContainer/AngularVelocityRow/HSlider");
        InitialTiltSlider = GetNode<HSlider>("TopVBox/VBoxContainer/InitialTiltRow/HSlider");
        TraceSizeSlider = GetNode<HSlider>("TopVBox/VBoxContainer/TraceSizeRow/HSlider");
        TimeStepSlider = GetNode<HSlider>("TopVBox/VBoxContainer/TimestepRow/HSlider");
    }

    public void ToggleControls(bool enabled)
    {
        VBoxContainer boxContainer = GetNode<VBoxContainer>("TopVBox/VBoxContainer");
        SetBoxEnabled(boxContainer, enabled);
    }

    private void SetBoxEnabled(BoxContainer boxContainer, bool enabled)
    {
        SetMouseRecursive(boxContainer, enabled);
        SetControlsRecursive(boxContainer, enabled);
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

    private void SetControlsRecursive(Control control, bool enabled)
    {
        foreach (Node child in control.GetChildren())
        {
            if (child is Control childControl)
            {
                if (childControl is BaseButton button)
                    button.Disabled = !enabled;
                else if (childControl is Slider slider)
                    slider.Editable = enabled;

                SetControlsRecursive(childControl, enabled);
            }
        }
    }
}
