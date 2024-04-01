using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Ui;



public partial class BetUi: Control
{
    public enum BetAction
    {
        Fold,
        Check,
        Call,
        Raise
    }
    
    [Signal]
    public delegate void ConfirmBetEventHandler(BetAction action);

    [Export] public PackedScene RaiseUiPrefab;
    
    [Export] public BaseButton FoldButton;
    [Export] public BaseButton CheckButton;
    [Export] public BaseButton CallButton;
    [Export] public BaseButton RaiseButton;

    public RaiseUi RaiseUi;

    private Match _match;

    public override void _Ready()
    {
        FoldButton.Pressed += Fold;
        var gameMgr = GetNode<GameMgr>("/root/GameMgr");
        _match = gameMgr.CurrentMatch;
    }

    public void Fold()
    {
        EmitSignal(SignalName.ConfirmBet, (int)BetAction.Fold);
    }

    public void Check()
    {
        EmitSignal(SignalName.ConfirmBet, (int)BetAction.Check);
    }

    public void Call()
    {
        EmitSignal(SignalName.ConfirmBet, (int)BetAction.Call);
        
    }

    public async void Raise()
    {
        RaiseUi.Setup(new Dictionary<string, object> ()
        {
            { "start_amount", _match.CurrentBetAmount }
        });
        await ToSignal(RaiseUi, RaiseUi.SignalName.ConfirmRaise);
        
        EmitSignal(SignalName.ConfirmBet, (int)BetAction.Call);
        
    }

    public void Setup(Dictionary<string, object> args)
    {
        throw new System.NotImplementedException();
    }
}