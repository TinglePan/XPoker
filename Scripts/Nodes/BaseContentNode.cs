using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public abstract partial class BaseContentNode<TNode, TContent> : Node2D, ISetup
    where TNode: BaseContentNode<TNode, TContent>
    where TContent: IContent<TNode, TContent>
{
    [Export] public Area2D Area;
    
    public ContentContainer<TNode, TContent> Container;
    public bool HasSetup { get; set; }

    public ObservableProperty<TContent> Content;
    public ObservableProperty<bool> IsFocused;
    public TweenControl TweenControl;

    public override void _Ready()
    {
        HasSetup = false;
        IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
        Content = new ObservableProperty<TContent>(nameof(Content), this, default);
        Content.DetailedValueChanged += OnContentChanged;
        Area.MouseEntered += OnMouseEnter;
        Area.MouseExited += OnMouseExit;
        TweenControl = new TweenControl();
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            var card = Content.Value;
            Content.Value = default;
        }
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Container = (ContentContainer<TNode, TContent>)args["container"];
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void TweenTransform(Vector2 position, float rotationDegrees, float tweenTime, Action callback = null)
    {
        var newTween = CreateTween().SetParallel();
        newTween.TweenProperty(this, "position", position, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        newTween.TweenProperty(this, "rotation_degrees", rotationDegrees, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        TweenControl.AddTween("transform", newTween, tweenTime, callback);
    }

    protected void OnMouseEnter()
    {
        GD.Print($"{this} OnMouseEnter");
        if (HasSetup)
        {
            IsFocused.Value = true;
        }
    }

    protected void OnMouseExit()
    {
        GD.Print($"{this} OnMouseExit");
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
        content.Node = (TNode)this;
    }

    protected virtual void OnContentDetached(TContent content)
    {
        content.Node = null;
    }

}