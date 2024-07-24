using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public partial class IconWithTextFallback: Node2D
{
    public class SetupArgs
    {
        public string DisplayName;
        public ObservableProperty<string> IconPath;
    }
    
    public Sprite2D Icon;
    public Label FallbackLabel;

    public ObservableProperty<string> DisplayName;
    public ObservableProperty<string> IconPath;

    public override void _Ready()
    {
        base._Ready();
        Icon = GetNode<Sprite2D>("Icon");
        FallbackLabel = GetNode<Label>("Label");
        FallbackLabel.Hide();
        DisplayName = new ObservableProperty<string>(nameof(DisplayName), this, null);
        DisplayName.DetailedValueChanged += OnDisplayNameChanged;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (IconPath != null)
        {
            IconPath.DetailedValueChanged -= OnIconPathChanged;
        }
    }

    public void Setup(object o)
    
    {
        var args = (SetupArgs)o;
        DisplayName.Value = args.DisplayName;
        ResetIconPath(args.IconPath);
    }

    public void ResetIconPath(ObservableProperty<string> iconPath)
    {
        if (IconPath != null)
        {
            IconPath.DetailedValueChanged -= OnIconPathChanged;
        }
        if (iconPath != null)
        {
            IconPath = iconPath;
            IconPath.DetailedValueChanged += OnIconPathChanged;
            IconPath.FireValueChangeEventsOnInit();
        }
    }
    
    protected void OnDisplayNameChanged(object sender, ValueChangedEventDetailedArgs<string> args)
    {
        if (args.NewValue != null)
        {
            FallbackLabel.Text = args.NewValue[0].ToString();
        }
        else
        {
            FallbackLabel.Text = "_";
        }
    }
    
    protected void OnIconPathChanged(object sender, ValueChangedEventDetailedArgs<string> args)
    {
        if (args.NewValue != null)
        {
            Icon.Texture = ResourceCache.Instance.Load<Texture2D>(args.NewValue);
        }
        if (Icon.Texture != null)
        {
            Icon.Show();
            FallbackLabel.Hide();
        }
        else
        {
            FallbackLabel.Show();
            Icon.Hide();
        }
    }
}