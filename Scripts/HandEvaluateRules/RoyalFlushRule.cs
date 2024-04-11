using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluateRules;

public class RoyalFlushRule: StraightFlushRule
{
    public override Enums.HandRank Rank => Enums.HandRank.RoyalFlush;
    
    public RoyalFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
    {
    }
    
    public override void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandRank,List<HandStrength>> calculatedHandStrengths, Enums.HandRank? forRank=null)
    {
        forRank ??= Rank;
        if (calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedHandStrength)) return;
        base.EvaluateAndRecord(cards, calculatedHandStrengths, base.Rank);
        if (calculatedHandStrengths.ContainsKey(base.Rank))
        {
            foreach (var handStrength in calculatedHandStrengths[base.Rank])
            {
                if (handStrength.PrimaryCards.Min().Rank == Enums.CardRank.Ten)
                {
                    UpgradeHandRank(handStrength, forRank.Value, calculatedHandStrengths);
                }
            }
        }
    }
}