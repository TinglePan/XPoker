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
        FastForwardAndFinish,
        Finish,
        InterruptEnsureOldCallback,
        Interrupt
    }
    
    public class ControlledTween: IEquatable<ControlledTween>
    {
        public string Tag;
        public ObservableProperty<Tween> Tween;
        public float Time;
        public ObservableProperty<Action> Callback;
        public TaskCompletionSource Complete;
        
        public ControlledTween(string tag, Tween tween, float time, Action callback = null)
        {
            Tag = tag;
            Tween = new ObservableProperty<Tween>(nameof(Tween), this, tween);
            Tween.DetailedValueChanged += TweenChanged;
            Time = time;
            Callback = new ObservableProperty<Action>(nameof(Callback), this, callback);
            Callback.DetailedValueChanged += CallbackChanged;
            Tween.FireValueChangeEventsOnInit();
            // Callback.FireValueChangeEventsOnInit();
        }

        public bool Equals(ControlledTween other)
        {
            return Tag == other?.Tag;
        }

        public bool IsRunning()
        {
            return Tween.Value != null && Tween.Value.IsRunning();
        }

        protected void TweenChanged(object sender, ValueChangedEventDetailedArgs<Tween> valueChangedEventDetailedArgs)
        {
            if (valueChangedEventDetailedArgs.OldValue != null)
            {
                valueChangedEventDetailedArgs.OldValue.Stop();
                if (Callback.Value != null)
                {
                    valueChangedEventDetailedArgs.OldValue.Finished -= Callback.Value;
                    valueChangedEventDetailedArgs.NewValue.Finished -= MarkComplete;
                }
            }
            if (valueChangedEventDetailedArgs.NewValue != null)
            {
                if (Callback.Value != null)
                {
                    valueChangedEventDetailedArgs.NewValue.Finished += Callback.Value;
                    valueChangedEventDetailedArgs.NewValue.Finished += MarkComplete;
                }
            }
        }

        protected void CallbackChanged(object sender, ValueChangedEventDetailedArgs<Action> valueChangedEventDetailedArgs)
        {
            if (Tween.Value != null)
            {
                if (valueChangedEventDetailedArgs.OldValue != null)
                {
                    Tween.Value.Finished -= valueChangedEventDetailedArgs.OldValue;
                }
                if (valueChangedEventDetailedArgs.NewValue != null)
                {
                    Tween.Value.Finished += valueChangedEventDetailedArgs.NewValue;
                }
            }
        }

        protected void MarkComplete()
        {
            Complete.TrySetResult();
        }
    }
    
    public Dictionary<string, ControlledTween> TweenMap;

    public TweenControl()
    {
        TweenMap = new Dictionary<string, ControlledTween>();
    }

    public void AddTween(string tag, Tween tween, float time, Action callback = null,
        ConflictTweenAction conflictAction = ConflictTweenAction.InterruptEnsureOldCallback)
    {
        if (TweenMap.ContainsKey(tag) && TweenMap[tag] is {} existingControlledTween)
        {
            if (existingControlledTween.IsRunning())
            {
                switch (conflictAction)
                {
                    case ConflictTweenAction.Interrupt:
                        existingControlledTween.Tween.Value.Stop();
                        break;
                    case ConflictTweenAction.InterruptEnsureOldCallback:
                        existingControlledTween.Tween.Value.Stop();
                        if (callback != existingControlledTween.Callback.Value)
                        {
                            existingControlledTween.Callback.Value?.Invoke();
                            existingControlledTween.Callback.Value = callback;
                        }
                        break;
                    case ConflictTweenAction.Finish:
                        existingControlledTween.Tween.Value.Stop();
                        existingControlledTween.Callback.Value?.Invoke();
                        if (callback != existingControlledTween.Callback.Value)
                        {
                            existingControlledTween.Callback.Value = callback;
                        }
                        break;
                    case ConflictTweenAction.FastForwardAndFinish:
                        existingControlledTween.Tween.Value.Pause();
                        existingControlledTween.Tween.Value.CustomStep(existingControlledTween.Time);
                        existingControlledTween.Tween.Value.Stop();
                        existingControlledTween.Callback.Value?.Invoke();
                        if (callback != existingControlledTween.Callback.Value)
                        {
                            existingControlledTween.Callback.Value = callback;
                        }
                        break;
                }
            }
            else
            {
                if (callback != null)
                {
                    existingControlledTween.Callback.Value = callback;
                }
            }
            existingControlledTween.Tween.Value = tween;
            existingControlledTween.Time = time;
        }
        else
        {
            var controlledTween = new ControlledTween(tag, tween, time, callback);
            TweenMap.Add(tag, controlledTween);
        }
    }

    public ControlledTween GetControlledTween(string tag)
    {
        return TweenMap.GetValueOrDefault(tag);
    }

    public Task WaitComplete(string tag)
    {
        return GetControlledTween(tag)?.Complete.Task;
    }

    public Tween GetTween(string tag)
    {
        var controlledTween = GetControlledTween(tag);
        return controlledTween?.Tween.Value;
    }
    
    public bool IsRunning(string tag)
    {
        return TweenMap.ContainsKey(tag) && TweenMap[tag].IsRunning();
    }

}