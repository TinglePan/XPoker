using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class BaseHandEvaluateRule
{
    public virtual void EvaluateAndRecord(List<BasePokerCard> cards,
        Dictionary<Enums.HandRank,List<CompletedHandStrength>> calculatedHandStrengths, Enums.HandRank? forRank=null)
    {
        forRank ??= Rank;
        if (calculatedHandStrengths != null &&
            calculatedHandStrengths.TryGetValue(forRank.Value, out var calculatedRes) && calculatedRes.Count > 0) return;
        var res = Evaluate(cards, forRank);
        if (calculatedHandStrengths!= null) calculatedHandStrengths[forRank.Value] = res;
    }
    
    protected virtual List<CompletedHandStrength> Evaluate(List<BasePokerCard> cards, Enums.HandRank? forRank=null)
    {
        forRank ??= Rank;
        var picks = Pick(cards);
        List<CompletedHandStrength> result = null;
        if (picks != null)
        {
            result = new List<CompletedHandStrength>();
            foreach (var pick in picks)
            {
                result.Add(new CompletedHandStrength(forRank.Value, pick,
                    GetPrimaryComparerCards(pick, cards), GetKickers(pick, cards)));
            }
        }
        return result;
    }
    
    public virtual Enums.HandRank Rank => Enums.HandRank.HighCard;
    
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

    protected virtual void UpgradeHandRank(CompletedHandStrength target, Enums.HandRank toRank,
        Dictionary<Enums.HandRank, List<CompletedHandStrength>> calculatedHandStrengths)
    {
        if (target.Rank != Rank) return;
        calculatedHandStrengths[target.Rank].Remove(target);
        target.Rank = toRank;
        if (!calculatedHandStrengths.ContainsKey(toRank))
        {
            calculatedHandStrengths[toRank] = new List<CompletedHandStrength>();
        }
        calculatedHandStrengths[toRank].Add(target);
    }
}