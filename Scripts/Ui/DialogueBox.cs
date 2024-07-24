using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public partial class DialogueBox: Container
{
    public ScrollContainer ScrollContainer;
    public Label TextWidget;

    public ObservableProperty<string> Content;
    
    public override void _Ready()
    {
        base._Ready();
        ScrollContainer = GetNode<ScrollContainer>("MarginContainer/ScrollContainer");
        TextWidget = GetNode<Label>("MarginContainer/ScrollContainer/LineEdit");
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
        ScrollContainer.ScrollVertical = (int)ScrollContainer.GetVScrollBar().MaxValue;
    }
}