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
        BlackJoker,
        RedJoker,
        RainbowJoker,
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
        RainbowJoker = 17,
    }

    public enum CardFace
    {
        Up,
        Down
    }

    public enum Direction1D
    {
        None,
        Left,
        Right
    }
    
    public enum HandTier
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

    public enum FactionId
    {
        Player,
        Enemy
    }

    public enum CardInteractions
    {
        None,
        Use,
        Activate,
        Seal
    }

    public enum TapDirection
    {
        Tapped,
        UnTapped
    }
}