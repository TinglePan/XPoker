using System;

namespace XCardGame.Scripts.Common.DataBinding;

public class DerivedObservableProperty<T> : IObservableProperty<T>, INotifyValueChanged<Lazy<T>>
{
    private Lazy<T> _value;
    private readonly Func<T> _valueGetter;
    
    public DerivedObservableProperty(
        string derivedPropertyName, Func<T> valueGetter,
        params INotifyValueChanged[] valueChangesToObserve)
    {
        Name = derivedPropertyName;
        _valueGetter = valueGetter;
        _value = new Lazy<T>(valueGetter);
        foreach (var valueChangeToObserve in valueChangesToObserve)
        {
            valueChangeToObserve.ValueChanged += (sender, e) => RefreshProperty();
        }
    }

    public event EventHandler<ValueChangedEventArgs> ValueChanged;
    public event EventHandler<ValueChangedEventDetailedArgs<Lazy<T>>> DetailedValueChanged;

    public T Value => _value.Value;
    
    public string Name { get; }

    public void RefreshProperty()
    {
        // Ensure we retrieve the value anew the next time it is requested
        var oldValue = _value;
        _value = new Lazy<T>(_valueGetter);
        ValueChanged?.Invoke(this, new ValueChangedEventArgs { PropertyName = Name });
        DetailedValueChanged?.Invoke(this, new ValueChangedEventDetailedArgs<Lazy<T>>
        {
            PropertyName = Name,
            OldValue = oldValue,
            NewValue = _value,
        });
    }

}
