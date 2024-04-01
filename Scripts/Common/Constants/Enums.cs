namespace XCardGame.Scripts.Common.Constants;

public static class Enums
{
    public enum Suit
    {
        None,
        Spades,
        Hearts,
        Diamonds,
        Clubs,
    }

    public enum Color
    {
        None,
        Red,
        Black,
    }
    
    public enum Rank
    {
        None,
        Ace = 1,
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
        BlackJoker = 14,
        RedJoker = 15,
    }

    public enum CardFace
    {
        Up,
        Down
    }
}