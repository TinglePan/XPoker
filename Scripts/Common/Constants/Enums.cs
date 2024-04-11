namespace XCardGame.Scripts.Common.Constants;

public static class Enums
{
    public enum CardSuit
    {
        None,
        Diamonds,
        Clubs,
        Hearts,
        Spades,
    }

    public enum CardColor
    {
        None,
        Red,
        Black,
    }
    
    public enum CardRank
    {
        None,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
        BlackJoker = 15,
        RedJoker = 16,
    }

    public enum CardFace
    {
        Up,
        Down
    }
    
    public enum HandRank
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    
    public enum PlayerAction
    {
        None,
        Fold,
        Check,
        Call,
        Raise
    }
}