using Godot;
using System;
using System.Collections.Generic;

public partial class SpinnerUI : Control
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

    public SpinBox EdgeLengthSpinBox { get; private set; }
    public SpinBox DensitySpinBox { get; private set; }
    public SpinBox AngularVelocitySpinBox { get; private set; }
    public SpinBox InitialTiltSpinBox { get; private set; }
    public SpinBox TraceSizeSpinBox { get; private set; }
    public SpinBox TimeStepSpinBox { get; private set; }


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

        EdgeLengthSpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/EdgeLengthRow/SpinBox");
        DensitySpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/DensityRow/SpinBox");
        AngularVelocitySpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/AngularVelocityRow/SpinBox");
        InitialTiltSpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/InitialTiltRow/SpinBox");
        TraceSizeSpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/TraceSizeRow/SpinBox");
        TimeStepSpinBox = GetNode<SpinBox>("TopVBox/VBoxContainer/TimestepRow/SpinBox");
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
                else if (childControl is SpinBox spinBox)
                    spinBox.Editable = enabled;

                SetControlsRecursive(childControl, enabled);
            }
        }
    }
}
