using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace XCardGame.Scripts.Brain.Ai;

public partial class BaseAi: BaseBrain
{
    public override Task AskForAction(Dictionary<string, object> context)
    {
        // GD.Print("BaseAi.OnAskForAction");
        var hand = (Hand)context["hand"];
        if (hand.RoundCallAmount == 0)
        {
            Player.Check();
        }
        else
        {
            Player.Call();
        }
        return Task.CompletedTask;
    }
}