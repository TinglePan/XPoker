using System.Collections.Generic;
using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public class HMenuButtons: HBoxContainer
{

    public class SetupArgs
    {
        public string Name;
        public List<(string, string)> ButtonTagsAndLabels;
        public int Separation;
    }
    
    public PackedScene MenuButtonPrefab;
    public Dictionary<string, Button> Buttons;
    
    public override void _Ready()
    {
        MenuButtonPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/MenuButton.tscn");
        Buttons = new Dictionary<string, Button>();
        
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }
    
    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        Name = args.Name;
        foreach (var (buttonTag, buttonLabel) in args.ButtonTagsAndLabels)
        {
            var button = (Button)Utils.InstantiatePrefab(MenuButtonPrefab, this);
            button.Text = buttonLabel;
            Buttons[buttonTag] = button;
        }
        if (args.Separation != 0)
        {
            Theme.SetConstant("separation", "HBoxContainer", args.Separation);
        }
    }
}