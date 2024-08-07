using System.Collections.Generic;
using XCardGame.Common;

namespace XCardGame;

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

    public static List<CardDef> StandardDeck(List<Enums.CardSuit> suits, List<Enums.CardRank> ranks)
    {
        
        var cardDefs = new List<CardDef>();
        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cardDefs.Add(new CardDef()
                {
                    BasePrice = 0,
                    DescriptionTemplate = "{} of {}",
                    ConcreteClassPath = "PokerCard",
                    Rank = rank,
                    Suit = suit,
                });
            }
        }

        return cardDefs;
    }

    public static List<CardDef> Standard52Deck()
    {
        var res = StandardDeck(_standardSuits, _standardRanks);
        return res;
    }
    
    public static List<CardDef> Enhanced52Deck()
    {
        var res = StandardDeck(_standardSuits, _standardRanks);
        return res;
    }

    public static List<CardDef> Short52Deck()
    {
        var res = StandardDeck(_standardSuits, _shortRanks);
        res.Add(CardDefs.ShortDeckRule);
        return res;
    }
    
    public static List<CardDef> EnhancedBlackDeck()
    {
        var res = StandardDeck(new List<Enums.CardSuit> { Enums.CardSuit.Spades, Enums.CardSuit.Clubs }, _standardRanks);
        res.Add(CardDefs.NerfFlush);
        return res;
    }
    
    public static List<CardDef> EnhancedRedDeck()
    {
        var res = StandardDeck(new List<Enums.CardSuit> { Enums.CardSuit.Hearts, Enums.CardSuit.Diamonds }, _standardRanks);
        res.Add(CardDefs.NerfFlush);
        return res;
    }
    
    public static List<CardDef> PlayerInitDeckDef = Standard52Deck();
}