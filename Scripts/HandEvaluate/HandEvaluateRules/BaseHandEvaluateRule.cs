using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class BaseHandEvaluateRule
{
    public virtual void EvaluateAndRecord(List<PokerCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHandStrengths, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHandStrengths != null &&
            calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedRes) && calculatedRes.Count > 0) return;
        var res = Evaluate(cards, forRank);
        if (calculatedHandStrengths!= null) calculatedHandStrengths[forRank.Value] = res;
    }
    
    protected virtual List<CompletedHand> Evaluate(List<PokerCard> cards, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        var picks = Pick(cards);
        List<CompletedHand> result = null;
        if (picks != null)
        {
            result = new List<CompletedHand>();
            foreach (var pick in picks)
            {
                result.Add(new CompletedHand(forRank.Value, pick,
                    GetPrimaryComparerCards(pick, cards), GetKickers(pick, cards)));
            }
        }
        return result;
    }
    
    public virtual Enums.HandTier Tier => Enums.HandTier.HighCard;
    
    protected virtual List<List<PokerCard>> Pick(List<PokerCard> cards)
    {
        var res = new List<List<PokerCard>>();
        return res;
    }
    
    protected virtual List<PokerCard> GetPrimaryComparerCards(List<PokerCard> pick, List<PokerCard> cards)
    {
        return new List<PokerCard> { pick.Max() };
    }
    
    protected virtual List<PokerCard> GetKickers(List<PokerCard> pick, List<PokerCard> cards)
    {
        var res = cards.Except(pick).OrderByDescending(c => c).ToList();
        return res;
    }

    protected virtual void UpgradeHandRank(CompletedHand target, Enums.HandTier toTier,
        Dictionary<Enums.HandTier, List<CompletedHand>> calculatedHandStrengths)
    {
        if (target.Tier != Tier) return;
        calculatedHandStrengths[target.Tier].Remove(target);
        target.Tier = toTier;
        if (!calculatedHandStrengths.ContainsKey(toTier))
        {
            calculatedHandStrengths[toTier] = new List<CompletedHand>();
        }
        calculatedHandStrengths[toTier].Add(target);
    }
}