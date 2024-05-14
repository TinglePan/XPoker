using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate;

public class CompletedHand: IComparable<CompletedHand>
{
    public Enums.HandTier Tier;
    public List<PokerCard> PrimaryCards;
    public List<PokerCard> PrimaryComparerCards;
    public List<PokerCard> Kickers;
    
    public CompletedHand(Enums.HandTier tier, List<PokerCard> primaryCards, List<PokerCard> primaryComparerCards,
        List<PokerCard> kickers)
    {
        Tier = tier;
        PrimaryCards = primaryCards;
        PrimaryComparerCards = primaryComparerCards;
        Kickers = kickers;
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
        for (var i = 0; i < PrimaryCards.Count; i++)
        {
            var compareRes = PrimaryCards[i].CompareTo(other.PrimaryCards[i], isSuitSecondComparer);
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