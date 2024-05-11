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
        if (Tier > other.Tier) return 1;
        if (Tier < other.Tier) return -1;
        for (var i = 0; i < PrimaryCards.Count; i++)
        {
            if (PrimaryCards[i].Rank.Value > other.PrimaryCards[i].Rank.Value) return 1;
            if (PrimaryCards[i].Rank.Value < other.PrimaryCards[i].Rank.Value) return -1;
        }
        if (Kickers != null)
        {
            for (var i = 0; i < Kickers.Count; i++)
            {
                if (Kickers[i].Rank.Value > other.Kickers[i].Rank.Value) return 1;
                if (Kickers[i].Rank.Value < other.Kickers[i].Rank.Value) return -1;
            }
        }
        return 0;
    }

    public Enums.CardRank LowestPrimaryCardRank => PrimaryCards.Count >= 1 ? PrimaryCards[0].Rank.Value : Enums.CardRank.None;
}