using System;
using System.Collections.Generic;
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
    
    public CompletedHand EvaluateBestHand(List<PokerCard> communityCards, List<PokerCard> holeCards)
    {
        CalculatedHands.Clear();
        var validHoleCards = holeCards.Where(x => !x.IsNegated.Value).ToList();
        var validCommunityCards = communityCards.Where(x => !x.IsNegated.Value).ToList();
        foreach (var cards in Utils.GetCombinationsWithXToYFromA(validHoleCards, validCommunityCards, 
                     CardCount, RequiredHoleCardCountMin, RequiredHoleCardCountMax))
        {
            Dictionary<Enums.HandTier, List<CompletedHand>> calculatedHandStrengths = new();
            foreach (var rule in Rules)
            {
                if (calculatedHandStrengths.TryGetValue(rule.Tier, out var handStrengths) && handStrengths.Count > 0) continue;
                rule.EvaluateAndRecord(cards, calculatedHandStrengths);
            }

            foreach (var (handRank, handStrengths) in calculatedHandStrengths)
            {
                if (!CalculatedHands.ContainsKey(handRank)) CalculatedHands[handRank] = new List<CompletedHand>();
                CalculatedHands[handRank].AddRange(handStrengths);
            }
        }

        var handRanksInDescendingOrder = ((Enums.HandTier[])Enum.GetValues(typeof(Enums.HandTier))).
            OrderByDescending(x => x);
        foreach (var handRank in handRanksInDescendingOrder)
        {
            if (CalculatedHands.TryGetValue(handRank, out var handStrengths) && handStrengths.Count > 0)
            {
                return handStrengths.OrderByDescending(x => x).First();
            }
        }
        GD.PrintErr("No hand rank rule matched. Not supposed to happen.");
        return null;
    }
    
    public (CompletedHand, CompletedHand) EvaluateBestHandsWithAndWithoutFaceDownCards(List<PokerCard> communityCards, List<PokerCard> holeCards)
    {
        var bestHandWithFaceDownCard = EvaluateBestHand(communityCards, holeCards);
        CalculatedHands.Clear();
        var bestHandWithoutFaceDownCard = EvaluateBestHand(communityCards.Where(x => x.Face.Value == Enums.CardFace.Up).ToList(), holeCards);
        return (bestHandWithFaceDownCard, bestHandWithoutFaceDownCard);
    }

    public int Compare(CompletedHand a, CompletedHand b)
    {
        return a.CompareTo(b, IsCompareHandTierOnly, IsSuitSecondComparer);
    }
}