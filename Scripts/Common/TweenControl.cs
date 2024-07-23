using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Common;

public class TweenControl
{
    public enum ConflictTweenAction
    {
        Interrupt,
        InterruptContinue,
        FastForward,
        FastForwardContinue,
    }
    
    public class ControlledTween
    {
        public string Tag;
        public int Priority;
        public ObservableProperty<Tween> Tween;
        public TaskCompletionSource Complete;
        public float Time;
        public Action Callback;
        
        public ControlledTween(string tag, Tween tween, float time, int priority = 0, Action callback = null)
        {
            Tag = tag;
            Priority = priority;
            Tween = new ObservableProperty<Tween>(nameof(Tween), this, tween);
            Tween.DetailedValueChanged += TweenChanged;
            Time = time;
            Callback += callback;
            Tween.FireValueChangeEventsOnInit();
            Complete = new TaskCompletionSource();
            // Callback.FireValueChangeEventsOnInit();
        }

        public void Reset()
        {
            Priority = 0;
            Tween.Value = null;
            Time = 0;
            Callback = null;
            Complete = new TaskCompletionSource();
        } 

        // public bool Equals(ControlledTween other)
        // {
        //     return Tag == other?.Tag;
        // }

        public bool IsRunning()
        {
            return Tween.Value != null && Tween.Value.IsRunning();
        }

        protected void TweenChanged(object sender, ValueChangedEventDetailedArgs<Tween> valueChangedEventDetailedArgs)
        {
            if (valueChangedEventDetailedArgs.OldValue != null)
            {
                valueChangedEventDetailedArgs.OldValue.Stop();
                valueChangedEventDetailedArgs.OldValue.Finished -= OnTweenFinished;
            }
            if (valueChangedEventDetailedArgs.NewValue != null)
            {
                valueChangedEventDetailedArgs.NewValue.Finished += OnTweenFinished;
            }
        }

        protected void OnTweenFinished()
        {
            Complete.TrySetResult();
            Callback?.Invoke();
            Reset();
        }
    }

    public Node Node;
    public Dictionary<string, ControlledTween> TweenMap;

    public TweenControl(Node node)
    {
        Node = node;
        TweenMap = new Dictionary<string, ControlledTween>();
    }

    public bool CanTween(string tag, int priority)
    {
        if (!TweenMap.ContainsKey(tag) || TweenMap[tag] == null) return true;
        if (TweenMap[tag].Priority <= priority) return true;
        return !TweenMap[tag].IsRunning();
    }
    
    public ControlledTween CreateTween(string tag, float time, int priority = 0, Action callback = null,
        ConflictTweenAction conflictAction = ConflictTweenAction.Interrupt)
    {
        if (!CanTween(tag, priority))
        {
            GD.Print($"Create tween for [{tag}] failed with priority {priority}.");
            return null;
        }
        var tween = Node.CreateTween();
        if (TweenMap.ContainsKey(tag) && TweenMap[tag] is {} existingControlledTween && existingControlledTween.Priority <= priority && existingControlledTween.IsRunning())
        {
            var oldTween = existingControlledTween.Tween.Value;
            switch (conflictAction)
            {
                case ConflictTweenAction.Interrupt:
                    oldTween.Stop();
                    break;
                case ConflictTweenAction.InterruptContinue:
                    oldTween.Stop();
                    time = (float)Mathf.Max(0, time - oldTween.GetTotalElapsedTime());
                    break;
                case ConflictTweenAction.FastForward:
                    oldTween.Pause();
                    oldTween.CustomStep(existingControlledTween.Time);
                    oldTween.Stop();
                    break;
                case ConflictTweenAction.FastForwardContinue:
                    oldTween.Pause();
                    oldTween.CustomStep(existingControlledTween.Time);
                    oldTween.Stop();
                    time = (float)Mathf.Max(0, time - oldTween.GetTotalElapsedTime());
                    break;
            }
            existingControlledTween.Tween.Value = tween;
            existingControlledTween.Time = time;
        }
        else
        {
            TweenMap[tag] = new ControlledTween(tag, tween, time, priority, callback);
        }
        return TweenMap[tag];
    }

    // public ControlledTween GetControlledTween(string tag)
    // {
    //     return TweenMap.GetValueOrDefault(tag);
    // }

    public Task WaitComplete(string tag)
    {
        return TweenMap.GetValueOrDefault(tag)?.Complete.Task;
    }

    public Task WaitTransformComplete()
    {
        return Task.WhenAll(new List<Task>
        {
            WaitComplete("position") ?? Task.CompletedTask,
            WaitComplete("rotation") ?? Task.CompletedTask,
        });
    }
    
    public bool IsRunning(string tag)
    {
        return TweenMap.ContainsKey(tag) && TweenMap[tag].IsRunning();
    }

}