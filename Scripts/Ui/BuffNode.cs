using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class BuffNode: Control, ISetup
{
    [Export]
    public TextureRect Icon;
    
    public ObservableProperty<BaseBuff> Buff;
    public BuffContainer Container;
    public ObservableProperty<bool> IsFocused;
    
    public override void _Ready()
    {
        Buff = new ObservableProperty<BaseBuff>(nameof(Buff), this, null);
        IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
    }
    
    public void Setup(Dictionary<string, object> args)
    {
        Buff.Value = (BaseBuff)args["buff"];
        Buff.DetailedValueChanged += OnBuffChanged;
        Container = (BuffContainer)args["container"];
    }

    protected void OnMouseEnter()
    {
        if (Buff is { Value: not null })
        {
            IsFocused.Value = true;
        }
    }

    protected void OnMouseExit()
    {
        if (Buff is { Value: not null })
        {
            IsFocused.Value = false;
        }
    }
    
    protected void OnBuffChanged(object sender, ValueChangedEventDetailedArgs<BaseBuff> args)
    {
        if (args.OldValue != null) OnBuffDetached(args.OldValue);
        if (args.NewValue != null) OnBuffAttached(args.NewValue);
    }
    
    protected void OnBuffAttached(BaseBuff buff)
    {
        // Implement attaching buff to the node
        buff.Node = this;
        buff.IconPath.DetailedValueChanged += OnIconPathChanged;
        buff.IconPath.FireValueChangeEventsOnInit();
    }

    protected void OnBuffDetached(BaseBuff buff)
    {
        // Implement detaching buff from the node
        buff.IconPath.DetailedValueChanged -= OnIconPathChanged;
    }

    protected void OnIconPathChanged(object sender, ValueChangedEventDetailedArgs<string> args)
    {
        if (args.NewValue != null)
        {
            Icon.Texture = ResourceCache.Instance.Load<Texture2D>(args.NewValue);
        }
        else
        {
            Icon.Texture = null;
        }
    }
}