using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public class DeckDef
{
    public List<BaseCardDef> CardDefs;
}


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
                cardDefs.Add(new BaseCardDef
                {
                    BasePrice = 0,
                    Rank = (Enums.CardRank)rank,
                    Suit = (Enums.CardSuit)suit
                });
            }
        }
        return new DeckDef
        {
            CardDefs = cardDefs
        };
    }
    
}