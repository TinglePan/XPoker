using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class PreFlopStrategy: BaseStrategy
{
    public static int[,] PreFlopHandTier = new int[13, 13]
    {
        { 0, 2, 2, 3, 5, 8, 10, 13, 14, 12, 14, 14, 17 },
        { 5, 1, 3, 3, 6, 10, 16, 19, 24, 25, 25, 26, 26 },
        { 8, 9, 1, 5, 6, 10, 19, 26, 28, 29, 29, 30, 31 },
        { 12, 14, 15, 2, 6, 11, 17, 27, 33, 35, 37, 37, 38 },
        { 18, 20, 22, 21, 4, 10, 16, 25, 31, 40, 40, 41, 41 },
        { 32, 35, 36, 34, 31, 7, 17, 24, 29, 38, 47, 47, 49 },
        { 39, 50, 53, 48, 43, 42, 9, 21, 27, 33, 40, 53, 54 },
        { 45, 57, 66, 64, 59, 55, 52, 12, 25, 28, 37, 45, 56 },
        { 51, 60, 71, 80, 74, 68, 61, 57, 16, 27, 29, 38, 49 },
        { 44, 63, 75, 82, 89, 83, 73, 65, 58, 20, 28, 32, 39 },
        { 46, 67, 76, 85, 90, 95, 88, 78, 70, 62, 23, 36, 41 },
        { 49, 67, 77, 86, 92, 96, 98, 93, 81, 72, 76, 23, 46 },
        { 54, 69, 79, 87, 94, 97, 99, 100, 95, 84, 86, 91, 24 }
    };

    protected static Curve RaiseMultiplierCurve = GD.Load<Curve>("res://Resources/RaiseMult2HandTier.tres");
    protected static Curve CheckMultiplierCurve = GD.Load<Curve>("res://Resources/CheckMult2HandTier.tres");
    protected static Curve FoldMultiplierCurve = GD.Load<Curve>("res://Resources/FoldMult2HandTier.tres");
    
    public PreFlopStrategy(ProbabilityActionAi ai, int weightBaseline) : base(ai, weightBaseline)
    {
    }
    
    public override bool CanTrigger()
    {
        if (Ai.Hand.RoundCount == 0)
        {
            return true;
        }

        return false;
    }
    
    public override void Trigger()
    {
        var player = Ai.Player;
        var hand = Ai.Hand;
        var sortedHoleCards = player.HoleCards.OfType<BasePokerCard>().OrderByDescending(x => x).ToList();
        var isOffSuit = sortedHoleCards[0].Suit != sortedHoleCards[1].Suit;
        var card1Index = CardRankToIndex(sortedHoleCards[0].Rank.Value);
        var card2Index = CardRankToIndex(sortedHoleCards[1].Rank.Value);
        var handTier = isOffSuit ? PreFlopHandTier[card2Index, card1Index] : PreFlopHandTier[card1Index, card2Index];
        
        var checkOrCallMultiplier = CheckMultiplierCurve.SampleBaked(handTier / 100.0f);
        var raiseMultiplier = RaiseMultiplierCurve.SampleBaked(handTier / 100.0f);
        var foldMultiplier = FoldMultiplierCurve.SampleBaked(handTier / 100.0f);
        GD.Print($"{this} change weights: {WeightBaseline * foldMultiplier}, {WeightBaseline * checkOrCallMultiplier}, {WeightBaseline * raiseMultiplier}");
        Ai.CheckOrCallWeight += (int)(WeightBaseline * checkOrCallMultiplier);
        Ai.RaiseWeight += (int)(WeightBaseline * raiseMultiplier);
        Ai.FoldWeight += (int)(WeightBaseline * foldMultiplier);
    }

    protected int CardRankToIndex(Enums.CardRank rank)
    {
        return (int)Enums.CardRank.Ace - (int)rank;
    }
}