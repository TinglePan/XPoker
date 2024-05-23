using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public class IconWithTextFallback: Sprite3D, ISetup
{
    [Export] public TextureRect Icon;
    [Export] public Label FallbackLabel;
    
    public bool HasSetup { get; set; }

    public string DisplayName;
    public ObservableProperty<string> IconPath;

    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        IconPath.DetailedValueChanged -= OnIconPathChanged;
    }

    public void Setup(Dictionary<string, object> args)
    {
        DisplayName = (string)args["displayName"];
        FallbackLabel.Text = DisplayName[0].ToString();
        ResetIconPath((ObservableProperty<string>)args["iconPath"]);
        HasSetup = true;
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

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
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