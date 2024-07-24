using System.Collections.Generic;
using System.Linq;
using XCardGame.Common;

namespace XCardGame;

public class RoyalFlushRule: StraightFlushRule
{
    public override Enums.HandTier Tier => Enums.HandTier.RoyalFlush;
    
    public RoyalFlushRule(int cardCount, bool allowWrap, bool allowShort) : base(cardCount, allowWrap, allowShort)
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
                if (handTier.PrimaryCards.Min().Rank.Value == Enums.CardRank.Ten)
                {
                    UpgradeHandTier(handTier, forRank.Value, calculatedHands);
                }
            }
        }
    }
}