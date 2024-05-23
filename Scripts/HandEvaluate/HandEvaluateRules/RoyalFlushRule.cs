﻿using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class RoyalFlushRule: StraightFlushRule
{
    public override Enums.HandTier Tier => Enums.HandTier.RoyalFlush;
    
    public RoyalFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
    {
    }
    
    public override void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHandStrengths, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedHandStrength)) return;
        base.EvaluateAndRecord(cards, calculatedHandStrengths, base.Tier);
        if (calculatedHandStrengths.ContainsKey(base.Tier))
        {
            foreach (var handStrength in calculatedHandStrengths[base.Tier])
            {
                if (handStrength.PrimaryCards.Min().Rank.Value == Enums.CardRank.Ten)
                {
                    UpgradeHandRank(handStrength, forRank.Value, calculatedHandStrengths);
                }
            }
        }
    }
}