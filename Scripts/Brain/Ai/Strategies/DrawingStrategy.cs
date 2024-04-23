using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class DrawingStrategy: BaseStrategy
{
    public DrawingStrategy(ProbabilityActionAi ai) : base(ai)
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
    }
}