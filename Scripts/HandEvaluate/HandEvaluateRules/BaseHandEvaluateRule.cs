using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class BaseHandEvaluateRule
{
    public virtual void EvaluateAndRecord(List<BasePokerCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHandStrengths, Enums.HandTier? forRank=null)
    {
        forRank ??= Tier;
        if (calculatedHandStrengths != null &&
            calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedRes) && calculatedRes.Count > 0) return;
        var res = Evaluate(cards, forRank);
        if (calculatedHandStrengths!= null) calculatedHandStrengths[forRank.Value] = res;
    }
    
    protected virtual List<CompletedHand> Evaluate(List<BasePokerCard> cards, Enums.HandTier? forRank=null)
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
    
    protected virtual List<List<BasePokerCard>> Pick(List<BasePokerCard> cards)
    {
        var res = new List<List<BasePokerCard>>();
        return res;
    }
    
    protected virtual List<BasePokerCard> GetPrimaryComparerCards(List<BasePokerCard> pick, List<BasePokerCard> cards)
    {
        return new List<BasePokerCard> { pick.Max() };
    }
    
    protected virtual List<BasePokerCard> GetKickers(List<BasePokerCard> pick, List<BasePokerCard> cards)
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