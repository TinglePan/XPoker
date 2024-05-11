using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class StraightFlushRule: StraightRule
{
    public override Enums.HandTier Tier => Enums.HandTier.StraightFlush;
    
    public StraightFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
    {
    }
    
    public override void EvaluateAndRecord(List<PokerCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHandStrengths, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedHandStrength)) return;
        base.EvaluateAndRecord(cards, calculatedHandStrengths, base.Tier);
        if (calculatedHandStrengths.ContainsKey(base.Tier))
        {
            foreach (var handStrength in calculatedHandStrengths[base.Tier])
            {
                if (handStrength.PrimaryCards.Select(card => card.Suit).Distinct().Count() == 1)
                {
                    UpgradeHandRank(handStrength, forRank.Value, calculatedHandStrengths);
                }
            }
        }
    }
}