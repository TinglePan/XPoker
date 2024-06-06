using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public abstract partial class BaseContentNode<TContent> : Node2D
    where TContent: IContent<TContent>
{
    public Area2D Area;

    public ObservableProperty<TContent> Content;
    public ObservableProperty<bool> IsFocused;
    public TweenControl TweenControl;

    public override void _Ready()
    {
        base._Ready();
        Area = GetNode<Area2D>("Area");
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
            Content.Value = default;
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
        IsFocused.Value = true;
    }

    protected void OnMouseExit()
    {
        GD.Print($"{this} OnMouseExit");
        IsFocused.Value = false;
    }

    protected void OnContentChanged(object sender, ValueChangedEventDetailedArgs<TContent> args)
    {
        if (args.OldValue != null) OnContentDetached(args.OldValue);
        if (args.NewValue != null) OnContentAttached(args.NewValue);
    }

    protected virtual void OnContentAttached(TContent content)
    {
        content.Nodes.Add(this);
    }

    protected virtual void OnContentDetached(TContent content)
    {
        content.Nodes.Remove(this);
    }
}

public abstract partial class BaseContentNode<TContentNode, TContent> : BaseContentNode<TContent>, ISetup
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContent>
{
    public ContentContainer<TContentNode, TContent> Container;
    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Container = (ContentContainer<TContentNode, TContent>)args["container"];
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
}