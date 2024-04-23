﻿using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class StraightFlushRule: StraightRule
{
    public override Enums.HandRank Rank => Enums.HandRank.StraightFlush;
    
    public StraightFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
    {
    }
    
    public override void EvaluateAndRecord(List<BasePokerCard> cards,
        Dictionary<Enums.HandRank,List<CompletedHandStrength>> calculatedHandStrengths, Enums.HandRank? forRank=null)
    {
        forRank ??= Rank;
        if (calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedHandStrength)) return;
        base.EvaluateAndRecord(cards, calculatedHandStrengths, base.Rank);
        if (calculatedHandStrengths.ContainsKey(base.Rank))
        {
            foreach (var handStrength in calculatedHandStrengths[base.Rank])
            {
                if (handStrength.PrimaryCards.Select(card => card.Suit).Distinct().Count() == 1)
                {
                    UpgradeHandRank(handStrength, forRank.Value, calculatedHandStrengths);
                }
            }
        }
    }
}