using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Common;

public class TweenControl
{
    public ObservableProperty<Tween> Tween;
    public float Time;
    public ObservableProperty<Action> Callback;

    public TweenControl()
    {
        Tween = new ObservableProperty<Tween>(nameof(Tween), this, null);
        Tween.DetailedValueChanged += TweenChanged;
        Time = 0;
        Callback = new ObservableProperty<Action>(nameof(Callback), this, null);
        Callback.DetailedValueChanged += CallbackChanged;
    }
    
    public bool IsRunning()
    {
        return Tween.Value != null && Tween.Value.IsRunning();
    }
    
    public void FastForwardAndStop()
    {
        if (Tween.Value != null)
        {
            Tween.Value.Pause();
            Tween.Value.CustomStep(Time);
            Tween.Value.Stop();
            Callback.Value?.Invoke();
        }
    }

    public void InterruptAsStop()
    {
        if (Tween.Value != null)
        {
            Tween.Value.Stop();
            Callback.Value?.Invoke();
        }
    }

    protected void TweenChanged(object sender, ValueChangedEventDetailedArgs<Tween> valueChangedEventDetailedArgs)
    {
        if (Callback.Value != null)
        {
            if (valueChangedEventDetailedArgs.OldValue != null)
            {
                valueChangedEventDetailedArgs.OldValue.Finished -= Callback.Value;
            }
            if (valueChangedEventDetailedArgs.NewValue != null)
            {
                valueChangedEventDetailedArgs.NewValue.Finished += Callback.Value;
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
}