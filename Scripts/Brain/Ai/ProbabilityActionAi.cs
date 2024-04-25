using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Brain.Ai;

public partial class ProbabilityActionAi: BaseAi
{
    public int FoldWeight;
    public int CheckOrCallWeight;
    public int RaiseWeight;
    public float RaiseAmountMultiplier;
    public (int, int) OpenRange;
    public (float, float) RaisePercentageRange;

    protected int BaseFoldWeight;
    protected int BaseCheckOrCallWeight;
    protected int BaseRaiseWeight;
    protected float BaseRaiseAmountMultiplier;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        FoldWeight = args.TryGetValue("foldWeight", out var arg) ? (int)arg : 1000;
        CheckOrCallWeight = args.TryGetValue("checkOrCallWeight", out arg) ? (int)arg : 1000;
        RaiseWeight = args.TryGetValue("raiseWeight", out arg) ? (int)arg : 1000;
        RaiseAmountMultiplier = 1.0f;
        BaseFoldWeight = FoldWeight;
        BaseCheckOrCallWeight = CheckOrCallWeight;
        BaseRaiseWeight = RaiseWeight;
        BaseRaiseAmountMultiplier = RaiseAmountMultiplier;
        var openRangeMin = (int)args["openRangeMin"];
        var openRangeMax = (int)args["openRangeMax"];
        OpenRange = (openRangeMin, openRangeMax);
        var raisePercentageRangeMin = (float)args["raisePercentageRangeMin"];
        var raisePercentageRangeMax = (float)args["raisePercentageRangeMax"];
        RaisePercentageRange = (raisePercentageRangeMin, raisePercentageRangeMax);
    }
    
    public override Task AskForAction(Dictionary<string, object> context)
    {
        // GD.Print("BaseAi.OnAskForAction");
        ResetWeights();
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
        GD.Print($"Fold/Check/Raise weight: {FoldWeight}/{CheckOrCallWeight}/{RaiseWeight}");
        GD.Print($"Rand: {random}");
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
            int raiseToAmount;
            if (Hand.HasOpened)
            {
                var raisePercentage = GameMgr.Rand.NextSingle() * (RaisePercentageRange.Item2 - RaisePercentageRange.Item1) + RaisePercentageRange.Item1;
                var raiseAmount = (int)(Hand.Pot.Total * raisePercentage);
                raiseToAmount = Mathf.Clamp(raiseAmount + Hand.RoundCallAmount, Hand.RoundMinRaiseToAmount, Player.NChipsInHand.Value + Hand.RoundCallAmount);
            }
            else
            {
                var raiseAmount = GameMgr.Rand.Next(OpenRange.Item1, OpenRange.Item2);
                raiseToAmount = Mathf.Clamp(raiseAmount, Hand.RoundMinRaiseToAmount, Player.NChipsInHand.Value);
            }
            Player.RaiseTo(raiseToAmount);
        }
    }
    
    protected void ResetWeights()
    {
        FoldWeight = BaseFoldWeight;
        CheckOrCallWeight = BaseCheckOrCallWeight;
        RaiseWeight = BaseRaiseWeight;
        RaiseAmountMultiplier = BaseRaiseAmountMultiplier;
    }
}