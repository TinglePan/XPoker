using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame;

public class BaseHandEvaluateRule
{
    public virtual void EvaluateAndRecord(List<BaseCard> cards,
        Dictionary<Enums.HandTier,List<CompletedHand>> calculatedHands, Enums.HandTier? forTier=null)
    {
        forTier ??= Tier;
        if (calculatedHands != null &&
            calculatedHands.TryGetValue(forTier.Value, out var calculatedRes) && calculatedRes.Count > 0) return;
        var res = Evaluate(cards, forTier);
        if (calculatedHands!= null && res != null) calculatedHands[forTier.Value] = res;
    }
    
    protected virtual List<CompletedHand> Evaluate(List<BaseCard> cards, Enums.HandTier? forTier=null)
    {
        forTier ??= Tier;
        var picks = Pick(cards);
        List<CompletedHand> result = null;
        if (picks != null)
        {
            result = new List<CompletedHand>();
            foreach (var pick in picks)
            {
                result.Add(new CompletedHand(forTier.Value, pick,
                    GetPrimaryComparerCards(pick, cards), GetKickers(pick, cards)));
            }
        }
        return result;
    }
    
    public virtual Enums.HandTier Tier => Enums.HandTier.HighCard;
    
    protected virtual List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        return null;
    }
    
    protected virtual List<BaseCard> GetPrimaryComparerCards(List<BaseCard> pick, List<BaseCard> cards)
    {
        return new List<BaseCard> { pick.Max() };
    }
    
    protected virtual List<BaseCard> GetKickers(List<BaseCard> pick, List<BaseCard> cards)
    {
        var res = cards.Except(pick).OrderByDescending(c => c).ToList();
        if (res.Count == 0 && pick.Count != 5)
        {
            GD.Print("kicker error: pick:");
            foreach (var card in pick)
            {
                GD.Print(card);
            }
            GD.Print("cards:");
            foreach (var card in cards)
            {
                GD.Print(card);
            }
            
        }
        return res;
    }

    protected virtual void UpgradeHandTier(CompletedHand target, Enums.HandTier toTier,
        Dictionary<Enums.HandTier, List<CompletedHand>> calculatedHands)
    {
        if (target.Tier != Tier) return;
        calculatedHands[target.Tier].Remove(target);
        target.Tier = toTier;
        if (!calculatedHands.ContainsKey(toTier))
        {
            calculatedHands[toTier] = new List<CompletedHand>();
        }
        calculatedHands[toTier].Add(target);
    }
}