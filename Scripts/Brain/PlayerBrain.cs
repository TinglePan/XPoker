using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Brain;

public partial class PlayerBrain: BaseBrain, ISetup
{
    private GameMgr _gameMgr;

    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
    }

    public override async Task AskForAction(Dictionary<string, object> context)
    {
        var actionUi = _gameMgr.OpenActionUi(context, OnConfirmAction);
        await ToSignal(actionUi, ActionUi.SignalName.ConfirmAction);
    }
    
    private void OnConfirmAction(Enums.PlayerAction action, int amount)
    {
        switch (action)
        {
            case Enums.PlayerAction.Fold:
                Player.Fold();
                break;
            case Enums.PlayerAction.Check:
                Player.Check();
                break;
            case Enums.PlayerAction.Call:
                Player.Call();
                break;
            case Enums.PlayerAction.Raise:
                Player.RaiseTo(amount);
                break;
        }
        _gameMgr.CloseActionUi();
    }
}