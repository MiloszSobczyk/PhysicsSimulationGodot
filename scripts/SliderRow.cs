using Godot;

[Tool]
public partial class SliderRow : HBoxContainer
{
    [Export]
    public string LabelText { get; set; } = "Value";

    private bool _useInteger = false;
    [Export]
    public bool UseInteger
    {
        get => _useInteger;
        set
        {
            if (_useInteger != value)
            {
                _useInteger = value;
                NotifyPropertyListChanged();
            }
        }
    }

    [Export]
    public float Min { get; set; } = 0;

    [Export]
    public float Max { get; set; } = 1;

    [Export]
    public float Step { get; set; } = 0.1f;

    private Label _label;
    private HSlider _slider;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _slider = GetNode<HSlider>("Slider");

        _label.Text = LabelText;

        _slider.MinValue = Min;
        _slider.MaxValue = Max;

        if (UseInteger)
        {
            _slider.Step = 1;
            _slider.MinValue = Mathf.Round(Min);
            _slider.MaxValue = Mathf.Round(Max);
        }
        else
        {
            _slider.Step = Step;
        }
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if (UseInteger)
        {
            string propName = property["name"].AsString();

            if (propName == PropertyName.Step)
            {
                var usage = property["usage"].As<PropertyUsageFlags>();
                property["usage"] = (int)(usage & ~PropertyUsageFlags.Editor);
            }

            if (propName == PropertyName.Min || propName == PropertyName.Max)
            {
                property["hint_string"] = "1";
            }
        }
    }

    public void Bind(System.Action<double> onValueChanged, double? initialValue = null)
    {
        if (_slider == null) return;

        if (initialValue.HasValue)
        {
            _slider.Value = initialValue.Value;
        }

    }

    public double GetValue()
    {
        return _slider.Value;
    }
}