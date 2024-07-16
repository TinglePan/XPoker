using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class IconWithTextFallback: Node2D
{
    public class SetupArgs
    {
        public string DisplayName;
        public ObservableProperty<string> IconPath;
    }
    
    public Sprite2D Icon;
    public Label FallbackLabel;

    public string DisplayName;
    public ObservableProperty<string> IconPath;

    public override void _Ready()
    {
        base._Ready();
        Icon = GetNode<Sprite2D>("Icon");
        FallbackLabel = GetNode<Label>("Label");
        FallbackLabel.Hide();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (IconPath != null)
        {
            IconPath.DetailedValueChanged -= OnIconPathChanged;
        }
    }

    public void Setup(SetupArgs args)
    {
        DisplayName = args.DisplayName;
        FallbackLabel.Text = DisplayName[0].ToString();
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