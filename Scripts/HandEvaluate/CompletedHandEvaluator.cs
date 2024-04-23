using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class CompletedHandEvaluator: BaseHandEvaluator
{
    // public Dictionary<string, object> Context;
    public Dictionary<Enums.HandRank, List<CompletedHandStrength>> CalculatedHands;

    public CompletedHandEvaluator(List<BasePokerCard> communityCards, int cardCount,
        int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null): base(communityCards, cardCount, requiredHoleCardCountMin, requiredHoleCardCountMax, rules)
    {
        Rules = rules ?? FiveCardHRules;
        CommunityCards = communityCards;
        CardCount = cardCount;
        RequiredHoleCardCountMin = requiredHoleCardCountMin;
        RequiredHoleCardCountMax = requiredHoleCardCountMax;
        CalculatedHands = new Dictionary<Enums.HandRank, List<CompletedHandStrength>>();
    }
    
    public CompletedHandStrength EvaluateBestHand(List<BasePokerCard> holeCards)
    {
        foreach (var cards in Utils.GetCombinationsWithXToYFromA(holeCards, CommunityCards, 
                     CardCount, RequiredHoleCardCountMin, RequiredHoleCardCountMax))
        {
            Dictionary<Enums.HandRank, List<CompletedHandStrength>> calculatedHandStrengths = new();
            foreach (var rule in Rules)
            {
                if (calculatedHandStrengths.TryGetValue(rule.Rank, out var handStrengths) && handStrengths.Count > 0) continue;
                rule.EvaluateAndRecord(cards, calculatedHandStrengths);
            }

            foreach (var (handRank, handStrengths) in calculatedHandStrengths)
            {
                if (!CalculatedHands.ContainsKey(handRank)) CalculatedHands[handRank] = new List<CompletedHandStrength>();
                CalculatedHands[handRank].AddRange(handStrengths);
            }
        }

        var handRanksInDescendingOrder = ((Enums.HandRank[])Enum.GetValues(typeof(Enums.HandRank))).
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

    public void Clear()
    {
        CalculatedHands.Clear();
    }
    
    public override List<float> EvaluateOdds(List<List<BasePokerCard>> holeCards)
    {
        throw new NotImplementedException();
    }

    public override float EvaluateAverageOdd(List<BasePokerCard> holeCards, int nTrials)
    {
        throw new NotImplementedException();
    }
}