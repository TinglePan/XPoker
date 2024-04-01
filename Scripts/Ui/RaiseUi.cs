using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;


public partial class RaiseUi: Control, ISetup
{
    [Signal]
    public delegate void ConfirmRaiseEventHandler(int amount);

    [Export] public Button ConfirmButton;

    private ObservableProperty<int> _startAmount;
    
    public override void _Ready()
    {
        ConfirmButton.Pressed += () => { EmitSignal(SignalName.ConfirmRaise, 100); };
        _startAmount = new ObservableProperty<int>(nameof(_startAmount), 0);
    }

    public void Setup(Dictionary<string, object> args)
    {
        _startAmount.Value = (int)args["start_amount"];
        
    }
}