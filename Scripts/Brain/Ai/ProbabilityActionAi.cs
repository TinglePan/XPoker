using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Brain.Ai;

public partial class ProbabilityActionAi: BaseAi
{
    public int FoldWeight;
    public int CheckOrCallWeight;
    public int RaiseWeight;
    public int RaiseToAmount;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        RaiseToAmount = Hand.RoundMinRaiseToAmount;
    }
    
    public override Task AskForAction(Dictionary<string, object> context)
    {
        // GD.Print("BaseAi.OnAskForAction");
        foreach (var strategy in Strategies)
        {
            if (strategy.CanTrigger())
            {
                strategy.Trigger();
            }
        }
        ChooseAction();
        return base.AskForAction(context);
    }

    public void ChooseAction()
    {
        var totalWeight = FoldWeight + CheckOrCallWeight + RaiseWeight;
        var random = GameMgr.Rand.Next(totalWeight);
        if (random < FoldWeight)
        {
            Player.Fold();
        }
        else if (random < FoldWeight + CheckOrCallWeight)
        {
            if (Hand.HasOpened)
            {
                Player.Call();
            }
            else
            {
                Player.Check();
            }
        }
        else
        {
            Player.RaiseTo(RaiseToAmount);
        }
    }
}