using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Brain.Ai.Strategies;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Brain.Ai;

public partial class BaseAi: BaseBrain
{

    public Hand Hand;
    public List<BaseStrategy> Strategies;


    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Hand = GameMgr.CurrentHand;
        Strategies = args.TryGetValue("strategies", out var arg) ? arg as List<BaseStrategy> : new List<BaseStrategy>();
    }

    public override Task AskForAction(Dictionary<string, object> context)
    {
        return Task.CompletedTask;
    }
}