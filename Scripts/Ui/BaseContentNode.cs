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
        TweenControl = new TweenControl(this);
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = default;
        }
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
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

    public virtual async Task AnimateTransform(Vector2 position, float rotationDegrees, float animationTime,
        int priority = 0, Action callback = null,
        TweenControl.ConflictTweenAction conflictTweenAction = TweenControl.ConflictTweenAction.Interrupt)
    {
        var tasks = new List<Task>();
        if (position != Position || TweenControl.IsRunning("position"))
        {
            var controlledTween = TweenControl.CreateTween("position", animationTime, priority, callback, conflictTweenAction);
            if (controlledTween != null)
            {
                var tween = controlledTween.Tween.Value;
                // tween.SetParallel();
                tween.TweenProperty(this, "position", position, controlledTween.Time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
                // tween.TweenProperty(this, "rotation_degrees", rotationDegrees, animationTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
                tasks.Add(TweenControl.WaitComplete("position"));
                callback = null;
                GD.Print($"animate transform {this} done {position}/{Position}");
            }
        }
        if (Math.Abs(RotationDegrees - rotationDegrees) > 0.1f || TweenControl.IsRunning("rotation"))
        {
            var controlledTween = TweenControl.CreateTween("rotation", animationTime, priority, callback, conflictTweenAction);
            if (controlledTween != null)
            {
                var tween = controlledTween.Tween.Value;
                tween.TweenProperty(this, "rotation_degrees", rotationDegrees, controlledTween.Time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
                tasks.Add(TweenControl.WaitComplete("rotation"));
            }
        }
        await Task.WhenAll(tasks);
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