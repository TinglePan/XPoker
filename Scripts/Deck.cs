using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public class Deck
{
    public List<BaseCard> CardList;

    public Deck()
    {
        CardList = new List<BaseCard>();
        HashSet<Enums.Rank> excludedRanks = new HashSet<Enums.Rank>()
        {
            Enums.Rank.None,
            Enums.Rank.BlackJoker,
            Enums.Rank.RedJoker
        };
        foreach (var suit in Enum.GetValues(typeof(Enums.Suit)))
        {
            if ((Enums.Suit)suit == Enums.Suit.None)
            {
                continue;
            }
            foreach (var rank in Enum.GetValues(typeof(Enums.Rank)))
            {
                if (excludedRanks.Contains((Enums.Rank)rank))
                {
                    continue;
                }

                BaseCard card = new BaseCard((Enums.Suit)suit, (Enums.Rank)rank, Enums.CardFace.Down);
                CardList.Add(card);
            }
        }
    }
}