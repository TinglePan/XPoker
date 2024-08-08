﻿using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.Common;

namespace XCardGame;

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
            case Enums.HandTier.None:
                break;
            case Enums.HandTier.FullHouse:
                var groups = PrimaryCards.GroupBy(card => card.Rank.Value, (rank, groupCards) => new
                {
                    Rank = rank,
                    Cards = groupCards,
                }).ToDictionary(x => x.Rank, x => x.Cards);
                PrimaryCards.Sort((x, y) =>
                {
                    var deltaCount = groups[x.Rank.Value].Count() - groups[y.Rank.Value].Count();
                    if (deltaCount != 0) return deltaCount;
                    return x.CompareTo(y, true);
                });
                break;
            default:
                PrimaryCards.Sort((x, y) => x.CompareTo(y, true));
                break;
        }
        Kickers?.Sort((x, y) => x.CompareTo(y, true));
    }

    public int CompareTo(CompletedHand other)
    {
        return CompareTo(other, false, false);
    }

    public int CompareTo(CompletedHand other, bool isCompareTierOnly, bool isSuitSecondComparer, List<Enums.HandTier> order = null)
    {
        if (order != null)
        {
            var thisIndex = order.IndexOf(Tier);
            var otherIndex = order.IndexOf(other.Tier);
            if (thisIndex > otherIndex) return -1;
            if (thisIndex < otherIndex) return 1;
        }
        else
        {
            if (Tier > other.Tier) return 1;
            if (Tier < other.Tier) return -1;
        }
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
}