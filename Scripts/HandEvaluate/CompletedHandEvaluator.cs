using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class CompletedHandEvaluator: BaseHandEvaluator
{
    // public Dictionary<string, object> Context;
    public Dictionary<Enums.HandTier, List<CompletedHand>> CalculatedHands;
    public bool IsCompareHandTierOnly;
    public bool IsSuitSecondComparer;

    public CompletedHandEvaluator(int cardCount,
        int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null): base(cardCount, requiredHoleCardCountMin, requiredHoleCardCountMax, rules)
    {
        Rules = rules ?? FiveCardHRules;
        CardCount = cardCount;
        RequiredHoleCardCountMin = requiredHoleCardCountMin;
        RequiredHoleCardCountMax = requiredHoleCardCountMax;
        CalculatedHands = new Dictionary<Enums.HandTier, List<CompletedHand>>();
        IsCompareHandTierOnly = false;
    }
    
    public CompletedHand EvaluateBestHand(List<BaseCard> communityCards, List<BaseCard> holeCards)
    {
        CalculatedHands.Clear();
        var validHoleCards = holeCards.Where(x => !x.Node.IsTapped).ToList();
        var validCommunityCards = communityCards.Where(x => !x.Node.IsTapped).ToList();
        // GD.Print($"valid Community cards: {validCommunityCards.Count}");
        // Profile.StartWatch("evaluate best hand 1");
        foreach (var cards in Utils.GetCombinationsWithXToYFromA(validHoleCards, validCommunityCards, 
                     CardCount, RequiredHoleCardCountMin, RequiredHoleCardCountMax))
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

        var handRanksInDescendingOrder = ((Enums.HandTier[])Enum.GetValues(typeof(Enums.HandTier))).
            OrderByDescending(x => x);
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

    public int Compare(CompletedHand a, CompletedHand b)
    {
        return a.CompareTo(b, IsCompareHandTierOnly, IsSuitSecondComparer);
    }
}