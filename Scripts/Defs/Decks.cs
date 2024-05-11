﻿using System;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public static class Decks
{
    public static Deck PlayerInitialDeck = LoadStandard52Deck();
    public static Deck OpponentInitialDeck = LoadStandard52Deck();

    private static Deck LoadStandard52Deck()
    {
        var deck = new Deck();
        foreach (var suit in Enum.GetValues(typeof(Enums.CardSuit)))
        {
            if ((Enums.CardSuit)suit == Enums.CardSuit.None)
            {
                continue;
            }
            foreach (var rank in Enum.GetValues(typeof(Enums.CardRank)))
            {
                if ((Enums.CardRank)rank == Enums.CardRank.None || (Enums.CardRank)rank == Enums.CardRank.BlackJoker || (Enums.CardRank)rank == Enums.CardRank.RedJoker)
                {
                    continue;
                }
                PokerCard pokerCard = new PokerCard(null, Enums.CardFace.Down, (Enums.CardSuit)suit, (Enums.CardRank)rank);
                deck.CardList.Add(pokerCard);
            }
        }

        return deck;
    }
}