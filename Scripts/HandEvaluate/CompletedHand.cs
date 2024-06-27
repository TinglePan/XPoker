using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class CompletedHand: IComparable<CompletedHand>
{
    public Enums.HandTier Tier;
    public List<BaseCard> PrimaryCards;
    public List<BaseCard> PrimaryComparerCards;
    public List<BaseCard> Kickers;
    
    public CompletedHand(Enums.HandTier tier, List<BaseCard> primaryCards, List<BaseCard> primaryComparerCards,
        List<BaseCard> kickers)
    {
        Tier = tier;
        PrimaryCards = primaryCards;
        PrimaryComparerCards = primaryComparerCards;
        Kickers = kickers;
    }

    public void Sort()
    {
        switch (Tier)
        {
            case Enums.HandTier.FullHouse:
                var groups = PrimaryCards.GroupBy(card => card.Rank.Value, (rank, groupCards) => new
                {
                    Rank = rank,
                    Cards = groupCards,
                }).ToDictionary(x => x.Rank, x => x.Cards);
                PrimaryCards.Sort((x, y) =>
                {
                    var deltaCount = groups[y.Rank.Value].Count() - groups[x.Rank.Value].Count();
                    if (deltaCount != 0) return deltaCount;
                    return y.CompareTo(x, true);
                });
                break;
            default:
                PrimaryCards.Sort((x, y) => y.CompareTo(x, true));
                break;
        }
        Kickers?.Sort((x, y) => y.CompareTo(x, true));
    }

    public int CompareTo(CompletedHand other)
    {
        return CompareTo(other, false, false);
    }

    public int CompareTo(CompletedHand other, bool isCompareTierOnly, bool isSuitSecondComparer)
    {
        if (Tier > other.Tier) return 1;
        if (Tier < other.Tier) return -1;
        if (isCompareTierOnly) return 0;
        for (var i = 0; i < PrimaryComparerCards.Count; i++)
        {
            var compareRes = PrimaryComparerCards[i].CompareTo(other.PrimaryComparerCards[i], isSuitSecondComparer);
            if (compareRes != 0) return compareRes;
        }
        if (Kickers != null)
        {
            for (var i = 0; i < Kickers.Count; i++)
            {
                var compareRes = Kickers[i].CompareTo(other.Kickers[i], isSuitSecondComparer);
                if (compareRes != 0) return compareRes;
            }
        }
        return 0;
    }

    public Enums.CardRank LowestPrimaryCardRank => PrimaryCards.Count >= 1 ? PrimaryCards[0].Rank.Value : Enums.CardRank.None;
}