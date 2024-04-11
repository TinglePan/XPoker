using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Ui;



public partial class ActionUi: BaseUi, ISetup
{
    
    [Signal]
    public delegate void ConfirmActionEventHandler(Enums.PlayerAction action, int amount);
    
    [Export] public RaiseUi RaiseUi;
    [Export] public Button FoldButton;
    [Export] public Button CheckButton;
    [Export] public Button CallButton;
    [Export] public Button RaiseButton;

    private GameMgr _gameMgr;
    


    public override void _Ready()
    {
        FoldButton.Pressed += Fold;
        CheckButton.Pressed += Check;
        CallButton.Pressed += Call;
        RaiseButton.Pressed += Raise;
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
    }

    public void Fold()
    {
        EmitSignal(SignalName.ConfirmAction, (int)Enums.PlayerAction.Fold, 0);
    }

    public void Check()
    {
        EmitSignal(SignalName.ConfirmAction, (int)Enums.PlayerAction.Check, 0);
    }

    public void Call()
    {
        EmitSignal(SignalName.ConfirmAction, (int)Enums.PlayerAction.Call, 0);
    }

    public async void Raise()
    {
        EmitSignal(SignalName.ConfirmAction, (int)Enums.PlayerAction.Raise, RaiseUi.Amount);
    }

    public void Setup(Dictionary<string, object> args)
    {
        RaiseUi.Setup(args);
        var hand = (Hand)args["hand"];
        var player = (PokerPlayer)args["player"];
        var callAmount = hand.RoundCallAmount;
        var canBet = !hand.RoundHasShortAllIn;
        
        if (callAmount == 0)
        {
            CallButton.Hide();
            CheckButton.Show();
        }
        else
        {
            CallButton.Show();
            CallButton.Text = player.WillBeAllIn(callAmount) ? "All in to call": $"{callAmount} to call";
            CheckButton.Hide();
        }

        if (!canBet)
        {
            RaiseButton.Hide();
        }
        else
        {
            RaiseButton.Show();
            var hasOpened = hand.RoundPreviousRaiseAmount > 0;
            var minRaiseAmount = hand.RoundMinRaiseToAmount - callAmount;
            RaiseButton.Text = hasOpened ? "Raise" : "Bet";
            if (player.WillBeAllIn(minRaiseAmount))
            {
                RaiseButton.Text = "All In to " + RaiseButton.Text;
            }
        }
    }
}