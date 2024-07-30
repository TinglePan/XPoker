using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame;

public class CompletedHandEvaluator: BaseHandEvaluator
{
    // public Dictionary<string, object> Context;
    public Dictionary<Enums.HandTier, List<CompletedHand>> CalculatedHands;
    public bool IsCompareHandTierOnly;
    public bool IsSuitSecondComparer;

    public CompletedHandEvaluator(int cardCount,
        int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null): base(cardCount, requiredHoleCardCountMin, requiredHoleCardCountMax, rules)
    {
        Rules = rules ?? FiveCardRules;
        CardCount = cardCount;
        RequiredHoleCardCountMin = requiredHoleCardCountMin;
        RequiredHoleCardCountMax = requiredHoleCardCountMax;
        CalculatedHands = new Dictionary<Enums.HandTier, List<CompletedHand>>();
        IsCompareHandTierOnly = false;
    }
    
    public CompletedHand EvaluateBestHand(List<BaseCard> validCommunityCards, List<BaseCard> validHoleCards, List<Enums.HandTier> handRanksInDescendingOrder)
    {
        CalculatedHands.Clear();
        // GD.Print($"valid Community cards: {validCommunityCards.Count}");
        // Profile.StartWatch("evaluate best hand 1");

        var cardCount = Mathf.Min(CardCount, validCommunityCards.Count + validHoleCards.Count);
        if (cardCount == 0)
        {
            return new CompletedHand(Enums.HandTier.None, null, null, null);
        }
        
        foreach (var cards in Utils.GetCombinationsWithXToYFromA(validHoleCards, validCommunityCards, 
                     cardCount, RequiredHoleCardCountMin, RequiredHoleCardCountMax))
        {
            // Profile.StartWatch("evaluate best hand 2");
            Dictionary<Enums.HandTier, List<CompletedHand>> calculatedHands = new();
            foreach (var rule in Rules)
            {
                if (calculatedHands.TryGetValue(rule.Tier, out var hands) && hands.Count > 0) continue;
                rule.EvaluateAndRecord(cards, calculatedHands);
            }

            // Profile.EndWatch("evaluate best hand 2", true, 1000);
            foreach (var (handRank, hands) in calculatedHands)
            {
                if (!CalculatedHands.ContainsKey(handRank)) CalculatedHands[handRank] = new List<CompletedHand>();
                CalculatedHands[handRank].AddRange(hands);
            }
        }
        // Profile.EndWatch("evaluate best hand 1", true, 1);

        foreach (var handRank in handRanksInDescendingOrder)
        {
            if (CalculatedHands.TryGetValue(handRank, out var hands) && hands.Count > 0)
            {
                return hands.OrderByDescending(x => x).First();
            }
        }
        GD.PrintErr("No hand rank rule matched. Not supposed to happen.");
        return null;
    }
    
    // public (CompletedHand, CompletedHand) EvaluateBestHandsWithAndWithoutFaceDownCards(List<BaseCard> communityCards, List<BaseCard> holeCards)
    // {
    //     var bestHandWithFaceDownCard = EvaluateBestHand(communityCards, holeCards);
    //     CalculatedHands.Clear();
    //     var bestHandWithoutFaceDownCard = EvaluateBestHand(
    //         communityCards.Where(x => x.Node.FaceDirection.Value == Enums.CardFace.Up).ToList(), holeCards);
    //     return (bestHandWithFaceDownCard, bestHandWithoutFaceDownCard);
    // }

    public int Compare(CompletedHand a, CompletedHand b, List<Enums.HandTier> handRanksInDescendingOrder)
    {
        return a.CompareTo(b, IsCompareHandTierOnly, IsSuitSecondComparer, handRanksInDescendingOrder);
    }
}