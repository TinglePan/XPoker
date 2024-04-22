using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class RoyalFlushRule: StraightFlushRule
{
    public override Enums.HandRank Rank => Enums.HandRank.RoyalFlush;
    
    public RoyalFlushRule(int cardCount, bool canWrap, bool allowAceLowStraight) : base(cardCount, canWrap, allowAceLowStraight)
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
                if (handStrength.PrimaryCards.Min().Rank.Value == Enums.CardRank.Ten)
                {
                    UpgradeHandRank(handStrength, forRank.Value, calculatedHandStrengths);
                }
            }
        }
    }
}