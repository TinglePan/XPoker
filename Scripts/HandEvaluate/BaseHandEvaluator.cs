﻿using System;
using System.Collections.Generic;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class BaseHandEvaluator
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
        new NOfAKindRule(Enums.HandTier.FourOfAKind, 4, NumericCardRanks),
        new NPlusMRule(Enums.HandTier.FullHouse, 3, 2, NumericCardRanks),
        new FlushRule(5, SuitWithColor),
        new StraightRule(5, false, true),
        new NOfAKindRule(Enums.HandTier.ThreeOfAKind, 3, NumericCardRanks),
        new NPlusMRule(Enums.HandTier.TwoPair, 2, 2, NumericCardRanks),
        new NOfAKindRule(Enums.HandTier.Pair, 2, NumericCardRanks),
        new NOfAKindRule(Enums.HandTier.HighCard, 1, NumericCardRanks),
    };
    
    
    public List<BaseHandEvaluateRule> Rules;
    public int CardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public BaseHandEvaluator(int cardCount, int requiredHoleCardCountMin, int requiredHoleCardCountMax,
        List<BaseHandEvaluateRule> rules = null)
    {
        Rules = rules ?? FiveCardHRules;
        CardCount = cardCount;
        RequiredHoleCardCountMin = requiredHoleCardCountMin;
        RequiredHoleCardCountMax = requiredHoleCardCountMax;
    }
}