using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.Card;


namespace XCardGame.Scripts.Defs;

public static class DeckDefs
{
    public static DeckDef PlayerInitDeckDef = Standard52Deck();
    public static DeckDef EnemyInitDeckDef = Standard52Deck();

    public static DeckDef Standard52Deck()
    {
        var cardDefs = new List<BaseCardDef>();
        foreach (var suit in Enum.GetValues(typeof(Enums.CardSuit)))
        {
            var unboxedSuit = (Enums.CardSuit)suit;
            if (unboxedSuit is Enums.CardSuit.None or Enums.CardSuit.Joker)
            {
                continue;
            }
            foreach (var rank in Enum.GetValues(typeof(Enums.CardRank)))
            {
                var unboxedRank = (Enums.CardRank)rank;
                if (unboxedRank is Enums.CardRank.None or Enums.CardRank.Joker)
                {
                    continue;
                }
                cardDefs.Add(new PokerCardDef()
                {
                    BasePrice = 0,
                    DescriptionTemplate = "{} of {}",
                    ConcreteClassPath = "PokerCard",
                    Rank = (Enums.CardRank)rank,
                    Suit = (Enums.CardSuit)suit,
                });
            }
        }
        return new DeckDef
        {
            CardDefs = cardDefs
        };
    }
    
}