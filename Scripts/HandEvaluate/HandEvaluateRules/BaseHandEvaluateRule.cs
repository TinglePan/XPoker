using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class BaseHandEvaluateRule
{
    public virtual void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHandStrengths, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHandStrengths != null &&
            calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedRes) && calculatedRes.Count > 0) return;
        var res = Evaluate(cards, forRank);
        if (calculatedHandStrengths!= null) calculatedHandStrengths[forRank.Value] = res;
    }
    
    protected virtual List<CompletedHand> Evaluate(List<BaseCard> cards, Enums.HandTier? forRank=null)
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
    
    protected virtual List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        var res = new List<List<BaseCard>>();
        return res;
    }
    
    protected virtual List<BaseCard> GetPrimaryComparerCards(List<BaseCard> pick, List<BaseCard> cards)
    {
        return new List<BaseCard> { pick.Max() };
    }
    
    protected virtual List<BaseCard> GetKickers(List<BaseCard> pick, List<BaseCard> cards)
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