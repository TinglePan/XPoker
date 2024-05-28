﻿using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class StraightFlushRule: StraightRule
{
    public override Enums.HandTier Tier => Enums.HandTier.StraightFlush;
    
    public StraightFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
    {
    }
    
    public override void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHands, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHands.TryGetValue(forRank.Value, out var calculatedHandTier)) return;
        base.EvaluateAndRecord(cards, calculatedHands, base.Tier);
        if (calculatedHands.ContainsKey(base.Tier))
        {
            foreach (var handTier in calculatedHands[base.Tier])
            {
                if (handTier.PrimaryCards.Select(card => card.Suit).Distinct().Count() == 1)
                {
                    UpgradeHandTier(handTier, forRank.Value, calculatedHands);
                }
            }
        }
    }
}