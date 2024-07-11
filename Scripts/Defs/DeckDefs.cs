using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.InteractCards.RuleCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;


namespace XCardGame.Scripts.Defs;

public static class DeckDefs
{
    
    private static List<Enums.CardSuit> _standardSuits = new List<Enums.CardSuit>
    {
        Enums.CardSuit.Spades,
        Enums.CardSuit.Hearts,
        Enums.CardSuit.Clubs,
        Enums.CardSuit.Diamonds,
    };
    
    private static List<Enums.CardRank> _standardRanks = new List<Enums.CardRank>
    {
        Enums.CardRank.Ace,
        Enums.CardRank.Two,
        Enums.CardRank.Three,
        Enums.CardRank.Four,
        Enums.CardRank.Five,
        Enums.CardRank.Six,
        Enums.CardRank.Seven,
        Enums.CardRank.Eight,
        Enums.CardRank.Nine,
        Enums.CardRank.Ten,
        Enums.CardRank.Jack,
        Enums.CardRank.Queen,
        Enums.CardRank.King,
    };

    private static List<Enums.CardRank> _shortRanks = new List<Enums.CardRank>()
    {
        Enums.CardRank.Six,
        Enums.CardRank.Seven,
        Enums.CardRank.Eight,
        Enums.CardRank.Nine,
        Enums.CardRank.Ten,
        Enums.CardRank.Jack,
        Enums.CardRank.Queen,
        Enums.CardRank.King,
    };

    public static DeckDef StandardDeck(List<Enums.CardSuit> suits, List<Enums.CardRank> ranks)
    {
        
        var cardDefs = new List<BaseCardDef>();
        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cardDefs.Add(new PokerCardDef()
                {
                    BasePrice = 0,
                    DescriptionTemplate = "{} of {}",
                    ConcreteClassPath = "PokerCard",
                    Rank = rank,
                    Suit = suit,
                });
            }
        }
        return new DeckDef
        {
            CardDefs = cardDefs
        };
    }
    
    public static DeckDef Standard52Deck()
    {
        var res = StandardDeck(_standardSuits, _standardRanks);
        res.CardDefs.Add(CardDefs.SpadesRule);
        res.CardDefs.Add(CardDefs.HeartsRule);
        res.CardDefs.Add(CardDefs.ClubsRule);
        res.CardDefs.Add(CardDefs.DiamondsRule);
        return res;
    }

    public static DeckDef Short52Deck()
    {
        var res = StandardDeck(_standardSuits, _shortRanks);
        res.CardDefs.Add(CardDefs.SpadesRule);
        res.CardDefs.Add(CardDefs.HeartsRule);
        res.CardDefs.Add(CardDefs.ClubsRule);
        res.CardDefs.Add(CardDefs.DiamondsRule);
        res.CardDefs.Add(CardDefs.ShortDeckRule);
        return res;
    }
    
    public static DeckDef EnhancedBlackDeck()
    {
        var res = StandardDeck(new List<Enums.CardSuit> { Enums.CardSuit.Spades, Enums.CardSuit.Clubs }, _standardRanks);
        res.CardDefs.Add(CardDefs.SpadesRule);
        res.CardDefs.Add(CardDefs.ClubsRule);
        res.CardDefs.Add(CardDefs.NerfFlush);
        return res;
    }
    
    public static DeckDef EnhancedRedDeck()
    {
        var res = StandardDeck(new List<Enums.CardSuit> { Enums.CardSuit.Hearts, Enums.CardSuit.Diamonds }, _standardRanks);
        res.CardDefs.Add(CardDefs.HeartsRule);
        res.CardDefs.Add(CardDefs.DiamondsRule);
        res.CardDefs.Add(CardDefs.NerfFlush);
        return res;
    }
    
    public static DeckDef PlayerInitDeckDef = Standard52Deck();
    public static DeckDef EnemyInitDeckDef = Standard52Deck();
    
}