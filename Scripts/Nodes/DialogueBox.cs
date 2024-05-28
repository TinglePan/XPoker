﻿using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public partial class DialogueBox: Container, IManagedNode
{
    [Export] public string Identifier { get; set; }
    [Export] public Label TextWidget;
    public GameMgr GameMgr { get; private set; }
    public SceneMgr SceneMgr { get; private set; }

    public ObservableProperty<string> Content;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        SceneMgr = GetNode<SceneMgr>("/root/SceneMgr");
        SceneMgr.Register(this);
        Content = new ObservableProperty<string>(nameof(Content), this, "");
        Content.DetailedValueChanged += OnContentChanged;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (Content != null)
        {
            Content.DetailedValueChanged -= OnContentChanged;
        }
    }
    
    protected void OnContentChanged(object sender, ValueChangedEventDetailedArgs<string> args)
    {
        // TODO: play text change animation
        TextWidget.Text = args.NewValue;
    }
}