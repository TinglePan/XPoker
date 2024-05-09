using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class CompletedHandAndOutsEvaluator: CompletedHandEvaluator
{
    public CompletedHandAndOutsEvaluator(int cardCount, int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null) : base(cardCount, requiredHoleCardCountMin, requiredHoleCardCountMax, rules)
    {
    }

    public CompletedHandAndOutsEvaluator(BaseHandEvaluator evaluator): base(evaluator.CardCount, evaluator.RequiredHoleCardCountMin, evaluator.RequiredHoleCardCountMax, evaluator.Rules)
    {
    }
    
    public (CompletedHand, CompletedHand) EvaluateBestHandWithAndWithoutOuts(List<BasePokerCard> communityCards, List<BasePokerCard> holeCards)
    {
        var faceUpCommunityCards = communityCards.Where(x => x.Face.Value == Enums.CardFace.Up).ToList();
        var bestHandWithoutFaceDownCommunityCards = base.EvaluateBestHand(faceUpCommunityCards, holeCards);
        var bestHand = base.EvaluateBestHand(communityCards, holeCards);
        return (bestHand, bestHandWithoutFaceDownCommunityCards);
    }
}