using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public abstract partial class BaseContentNode<TContent> : Node2D, ISetup
    where TContent: IContent<TContent>
{
    
    public bool HasSetup { get; set; }
    
    public bool HasPhysics;
    public Area2D Area;
    public Action<BaseContentNode<TContent>> OnPressed;
    public Action<BaseContentNode<TContent>> OnHover;
    public Action<BaseContentNode<TContent>> OnUnHover;
    public ObservableProperty<TContent> Content;
    public bool IsHovered;
    public TweenControl TweenControl;

    public override void _Ready()
    {
        base._Ready();
        IsHovered = false;
        Content = new ObservableProperty<TContent>(nameof(Content), this, default);
        Content.DetailedValueChanged += OnContentChanged;
        TweenControl = new TweenControl();
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = default;
        }
    }
    

    public virtual void Setup(Dictionary<string, object> args)
    {
        HasPhysics = (bool)args["hasPhysics"];
        if (HasPhysics)
        {
            Area = GetNode<Area2D>("Area");
            Area.MouseEntered += OnMouseEnter;
            Area.MouseExited += OnMouseExit;
            Area.InputEvent += OnInput;
        }
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void TweenTransform(Vector2 position, float rotationDegrees, float tweenTime, Action callback = null,
        TweenControl.ConflictTweenAction conflictTweenAction = TweenControl.ConflictTweenAction.Interrupt)
    {
        var newTween = CreateTween().SetParallel();
        newTween.TweenProperty(this, "position", position, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        newTween.TweenProperty(this, "rotation_degrees", rotationDegrees, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        TweenControl.AddTween("transform", newTween, tweenTime, callback, conflictTweenAction);
    }

    protected void OnMouseEnter()
    {
        // GD.Print($"{this} OnMouseEnter");
        IsHovered = true;
        OnHover?.Invoke(this);
    }

    protected void OnMouseExit()
    {
        // GD.Print($"{this} OnMouseExit");
        IsHovered = false;
        OnUnHover?.Invoke(this);
    }

    protected void OnInput(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                GD.Print($"On mouse pressed {Content.Value}");
                OnPressed?.Invoke(this);
            }
        }
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

public abstract partial class BaseContentNode<TContentNode, TContent> : BaseContentNode<TContent>
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContent>
{
    public ObservableProperty<ContentContainer<TContentNode, TContent>> Container;

    public override void _Ready()
    {
        base._Ready();
        Container = new ObservableProperty<ContentContainer<TContentNode, TContent>>(nameof(Container), this, null);
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Container.Value = (ContentContainer<TContentNode, TContent>)args["container"];
    }
}