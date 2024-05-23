using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public abstract partial class BaseContentNode<TNode, TContent> : Node3D, ISetup
    where TNode: BaseContentNode<TNode, TContent>
    where TContent: IContent<TNode, TContent>
{
    [Export] public Area3D Area;
    
    public BaseContentContainer<TNode, TContent> Container;
    public bool HasSetup { get; set; }

    public ObservableProperty<TContent> Content;
    public ObservableProperty<bool> IsFocused;

    public override void _Ready()
    {
        HasSetup = false;
        IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
        Content = new ObservableProperty<TContent>(nameof(Content), this, default);
        Content.DetailedValueChanged += OnContentChanged;
        Area.MouseEntered += OnMouseEnter;
        Area.MouseExited += OnMouseExit;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Container = (BaseContentContainer<TNode, TContent>)args["container"];
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    protected void OnMouseEnter()
    {
        if (HasSetup)
        {
            IsFocused.Value = true;
        }
    }

    protected void OnMouseExit()
    {
        if (HasSetup)
        {
            IsFocused.Value = false;
        }
    }

    protected void OnContentChanged(object sender, ValueChangedEventDetailedArgs<TContent> args)
    {
        if (args.OldValue != null) OnContentDetached(args.OldValue);
        if (args.NewValue != null) OnContentAttached(args.NewValue);
    }

    protected virtual void OnContentAttached(TContent content)
    {
        // Implement attaching buff to the node
        content.Node = (TNode)this;
    }

    protected virtual void OnContentDetached(TContent content)
    {
        content.Node = null;
        // Implement detaching buff from the node
    }
}