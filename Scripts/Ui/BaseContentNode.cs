using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public abstract partial class BaseContentNode : Node2D
{
    public class SetupArgs
    {
        public bool HasPhysics;
        public BaseContentContainer Container;
        public IContent Content;
    }
    
    public Area2D Area;
    public Action<BaseContentNode, MouseButton> OnMousePressed;
    public Action<BaseContentNode> OnHover;
    public Action<BaseContentNode> OnUnHover;

    public BaseContentContainer PreviousContainer;
    public ObservableProperty<BaseContentContainer> CurrentContainer;
    public ObservableProperty<IContent> Content;
    public TweenControl TweenControl;

    public override void _Ready()
    {
        Content = new ObservableProperty<IContent>(nameof(Content), this, default);
        Content.DetailedValueChanged += OnContentChanged;
        CurrentContainer = new ObservableProperty<BaseContentContainer>(nameof(CurrentContainer), this, null);
        TweenControl = new TweenControl();
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = default;
        }
    }

    public virtual void Setup(SetupArgs args)
    {
        if (args.HasPhysics)
        {
            Area = GetNode<Area2D>("Area");
            Area.MouseEntered += OnMouseEnter;
            Area.MouseExited += OnMouseExit;
            Area.InputEvent += OnInput;
        }
        PreviousContainer = null;
        CurrentContainer.Value = args.Container;
        Content.Value = args.Content;
    }

    public async Task AnimateTransform(Vector2 position, float rotationDegrees, float animationTime,
        Action callback = null,
        TweenControl.ConflictTweenAction conflictTweenAction = TweenControl.ConflictTweenAction.Interrupt)
    {
        var newTween = CreateTween().SetParallel();
        newTween.TweenProperty(this, "position", position, animationTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        newTween.TweenProperty(this, "rotation_degrees", rotationDegrees, animationTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        TweenControl.AddTween("transform", newTween, animationTime, callback, conflictTweenAction);
        await TweenControl.WaitComplete("transform");
    }

    protected void OnMouseEnter()
    {
        // GD.Print($"{this} OnMouseEnter");
        OnHover?.Invoke(this);
    }

    protected void OnMouseExit()
    {
        // GD.Print($"{this} OnMouseExit");
        OnUnHover?.Invoke(this);
    }

    protected void OnInput(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton { Pressed: true } mouseButton)
        {
            GD.Print($"On mouse pressed {Content.Value}");
            OnMousePressed?.Invoke(this, mouseButton.ButtonIndex);
        }
    }

    protected void OnContentChanged(object sender, ValueChangedEventDetailedArgs<IContent> args)
    {
        if (args.OldValue != null) OnContentDetached(args.OldValue);
        if (args.NewValue != null) OnContentAttached(args.NewValue);
    }

    protected virtual void OnContentAttached(IContent content)
    {
        content.Nodes.Add(this);
    }

    protected virtual void OnContentDetached(IContent content)
    {
        content.Nodes.Remove(this);
    }
}