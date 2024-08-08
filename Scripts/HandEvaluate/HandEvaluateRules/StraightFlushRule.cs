using System.Collections.Generic;
using System.Linq;
using XCardGame.Common;

namespace XCardGame;

public class StraightFlushRule: StraightRule
{
    public override Enums.HandTier Tier => Enums.HandTier.StraightFlush;
    
    public StraightFlushRule(int cardCount, bool allowWrap, bool allowShort) : base(cardCount, allowWrap, allowShort)
    {
    }
    
    public override void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHands, Enums.HandTier? forTier=null)
    {
        forTier ??= Tier;
        if (calculatedHands.TryGetValue(forTier.Value, out var calculatedHandTier)) return;
        base.EvaluateAndRecord(cards, calculatedHands, base.Tier);
        if (calculatedHands.ContainsKey(base.Tier))
        {
            foreach (var handTier in calculatedHands[base.Tier])
            {
                if (handTier.PrimaryCards.Select(card => card.Suit).Distinct().Count() == 1)
                {
                    UpgradeHandTier(handTier, forTier.Value, calculatedHands);
                }
            }
        }
    }
}