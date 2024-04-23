using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class DrawingStrategy: BaseStrategy
{
    
    protected static Curve RaiseMultiplierCurve = GD.Load<Curve>("res://Resources/RaiseMult2HandTier.tres");
    protected static Curve CheckMultiplierCurve = GD.Load<Curve>("res://Resources/CheckMult2HandTier.tres");
    protected static Curve FoldMultiplierCurve = GD.Load<Curve>("res://Resources/FoldMult2HandTier.tres");
    
    public DrawingStrategy(ProbabilityActionAi ai, int weightBaseline) : base(ai, weightBaseline)
    {
    }
    
    public override bool CanTrigger()
    {
        if (Ai.Hand.RoundCount > 0 && Ai.Hand.RoundCount < Configuration.RiverRoundIndex)
        {
            return true;
        }

        return false;
    }

    public override void Trigger()
    {
        var player = Ai.Player;
        var hand = Ai.Hand;
        var holeCards = player.HoleCards.OfType<BasePokerCard>().ToList();
        var communityCards = hand.CommunityCards.OfType<BasePokerCard>().ToList();

        var evaluator = new CompletedHandEvaluator(communityCards, 5,
            0, 2);
        var avgOdd = evaluator.EvaluateAverageOdd(holeCards, 100);
        Ai.CheckOrCallWeight += (int)(WeightBaseline * CheckMultiplierCurve.Sample(avgOdd));
        Ai.RaiseWeight += (int)(WeightBaseline * RaiseMultiplierCurve.Sample(avgOdd));
        Ai.FoldWeight += (int)(WeightBaseline * FoldMultiplierCurve.Sample(avgOdd));
    }
}