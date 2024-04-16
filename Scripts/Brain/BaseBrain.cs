using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Brain;

public partial class BaseBrain: Node, ISetup
{
    protected PokerPlayer Player;
    
    public virtual Task AskForAction(Dictionary<string, object> context)
    {
        // GD.Print("BaseBrain.OnAskForAction");
        return Task.CompletedTask;
    }

    public void Setup(Dictionary<string, object> args)
    {
        Player = (PokerPlayer)args["player"];
    }
}