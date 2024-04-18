using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Brain;

public partial class PlayerBrain: BaseBrain
{
    private GameMgr _gameMgr;
    private ActionUi _actionUi;

    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        _actionUi = _gameMgr.ActionUi;
        _actionUi.ConfirmAction += OnConfirmAction;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _actionUi.ConfirmAction -= OnConfirmAction;
    }

    public override async Task AskForAction(Dictionary<string, object> context)
    {
        _gameMgr.OpenUi(_gameMgr.ActionUi, context);
        await ToSignal(_actionUi, ActionUi.SignalName.ConfirmAction);
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
        _gameMgr.CloseUi(_gameMgr.ActionUi);
    }
}