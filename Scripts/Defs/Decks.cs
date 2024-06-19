using System;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public static class Decks
{
    public static Deck PlayerInitialDeck = LoadStandard52Deck();
    public static Deck EnemyInitialDeck = LoadStandard52Deck();

    private static Deck LoadStandard52Deck()
    {
        var deck = new Deck();
        foreach (var suit in Enum.GetValues(typeof(Enums.CardSuit)))
        {
            var unboxedSuit = (Enums.CardSuit)suit;
            if (unboxedSuit == Enums.CardSuit.None || unboxedSuit == Enums.CardSuit.BlackJoker || unboxedSuit == Enums.CardSuit.RedJoker || unboxedSuit == Enums.CardSuit.RainbowJoker)
            {
                continue;
            }
            foreach (var rank in Enum.GetValues(typeof(Enums.CardRank)))
            {
                var unboxedRank = (Enums.CardRank)rank;
                if (unboxedRank == Enums.CardRank.None || unboxedRank == Enums.CardRank.BlackJoker || unboxedRank == Enums.CardRank.RedJoker || unboxedRank == Enums.CardRank.RainbowJoker)
                {
                    continue;
                }
                PokerCard pokerCard = new PokerCard(new BaseCardDef()
                {
                    BasePrice = 0,
                    Rank = (Enums.CardRank)rank,
                    Suit = (Enums.CardSuit)suit
                });
                deck.CardList.Add(pokerCard);
            }
        }

        return deck;
    }
}