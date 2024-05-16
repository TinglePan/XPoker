using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class DialogueBox: Container, IManagedUi
{
    [Export] public string Identifier { get; set; }
    [Export] public Label TextWidget;
    public GameMgr GameMgr { get; set; }
    public UiMgr UiMgr { get; set; }

    public ObservableProperty<string> Content;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        UiMgr = GetNode<UiMgr>("/root/UiMgr");
        UiMgr.Register(this);
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
        TextWidget.Text = args.NewValue;
    }
}