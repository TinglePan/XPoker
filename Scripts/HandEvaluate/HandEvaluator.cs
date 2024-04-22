using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class HandEvaluator
{
    public static List<Enums.CardRank> NumericCardRanks = new ()
    {
        Enums.CardRank.Two,
        Enums.CardRank.Three,
        Enums.CardRank.Four,
        Enums.CardRank.Five,
        Enums.CardRank.Six,
        Enums.CardRank.Seven,
        Enums.CardRank.Eight,
        Enums.CardRank.Nine,
        Enums.CardRank.Ten,
        Enums.CardRank.Jack,
        Enums.CardRank.Queen,
        Enums.CardRank.King,
        Enums.CardRank.Ace
    };

    public static List<Enums.CardSuit> SuitWithColor = new()
    {
        Enums.CardSuit.Diamonds,
        Enums.CardSuit.Clubs,
        Enums.CardSuit.Hearts,
        Enums.CardSuit.Spades,
    };

    public static List<BaseHandEvaluateRule> FiveCardHRules = new()
    {
        new RoyalFlushRule(5, false, true),
        new StraightFlushRule(5, false, false),
        new NOfAKindRule(Enums.HandRank.FourOfAKind, 4, NumericCardRanks),
        new NPlusMRule(Enums.HandRank.FullHouse, 3, 2, NumericCardRanks),
        new FlushRule(5, SuitWithColor),
        new StraightRule(5, false, true),
        new NOfAKindRule(Enums.HandRank.ThreeOfAKind, 3, NumericCardRanks),
        new NPlusMRule(Enums.HandRank.TwoPair, 2, 2, NumericCardRanks),
        new NOfAKindRule(Enums.HandRank.Pair, 2, NumericCardRanks),
        new NOfAKindRule(Enums.HandRank.HighCard, 1, NumericCardRanks),
    };
    
    public List<BaseHandEvaluateRule> Rules;
    public List<BasePokerCard> CommunityCards;
    public int CardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    // public Dictionary<string, object> Context;
    
    public Dictionary<Enums.HandRank, List<CompletedHandStrength>> CalculatedHands;

    public HandEvaluator(List<BasePokerCard> communityCards, int cardCount,
        int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null)
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

    public List<float> CompareDrawingHands(List<List<BasePokerCard>> holeCards)
    {
        throw new NotImplementedException();
    }
}