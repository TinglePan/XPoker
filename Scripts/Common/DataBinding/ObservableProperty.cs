using System;

namespace XCardGame.Scripts.Common.DataBinding;

public class ObservableProperty<T>: IObservableProperty<T>
{
    private T _value;
    
    public ObservableProperty(string name, T initialValue)
    {
        Name = name;
        _value = initialValue;
    }
    
    public event EventHandler<ValueChangedEventArgs> ValueChanged = delegate { };
    public event EventHandler<ValueChangedEventDetailedArgs<T>> DetailedValueChanged = delegate { };

    public string Name { get; }

    public T Value
    {
        get => _value;
        set
        {
            if (Equals(value, _value)) return;
            var oldValue = _value;
            _value = value;
            ValueChanged?.Invoke(this, new ValueChangedEventArgs {PropertyName = Name});
            DetailedValueChanged?.Invoke(this, new ValueChangedEventDetailedArgs<T>
            {
                PropertyOwner = this,
                PropertyName = Name,
                OldValue = oldValue,
                NewValue = value
            });
        }
    }

    public void FireValueChangeEvents()
    {
        ValueChanged?.Invoke(this, new ValueChangedEventArgs {PropertyName = Name});
        DetailedValueChanged?.Invoke(this, new ValueChangedEventDetailedArgs<T>
        {
            PropertyOwner = this,
            PropertyName = Name,
            OldValue = Value,
            NewValue = Value
        });
    }

}