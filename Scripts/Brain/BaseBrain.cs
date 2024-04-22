using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Brain;

public partial class BaseBrain: Node, ISetup
{
    public GameLogic.PokerPlayer Player;
    protected GameMgr GameMgr;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
    }
    
    public virtual Task AskForAction(Dictionary<string, object> context)
    {
        // GD.Print("BaseBrain.OnAskForAction");
        return Task.CompletedTask;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Player = (GameLogic.PokerPlayer)args["player"];
    }
}