using System;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards;

public static class CardExtensions
{
    public static string ToFriendlyString(this Enums.Suit cardSuit)
    {
        switch (cardSuit)
        {
            case Enums.Suit.Clubs:
                return "\u2663"; // ♣
            case Enums.Suit.Diamonds:
                return "\u2666"; // ♦
            case Enums.Suit.Hearts:
                return "\u2665"; // ♥
            case Enums.Suit.Spades:
                return "\u2660"; // ♠
            default:
                return "\u0000"; // �
        }
    }
    
    public static string ToFriendlyString(this Enums.Rank cardRank)
    {
        switch (cardRank)
        {
            case Enums.Rank.Two:
                return "2";
            case Enums.Rank.Three:
                return "3";
            case Enums.Rank.Four:
                return "4";
            case Enums.Rank.Five:
                return "5";
            case Enums.Rank.Six:
                return "6";
            case Enums.Rank.Seven:
                return "7";
            case Enums.Rank.Eight:
                return "8";
            case Enums.Rank.Nine:
                return "9";
            case Enums.Rank.Ten:
                return "10";
            case Enums.Rank.Jack:
                return "J";
            case Enums.Rank.Queen:
                return "Q";
            case Enums.Rank.King:
                return "K";
            case Enums.Rank.Ace:
                return "A";
            case Enums.Rank.BlackJoker:
            case Enums.Rank.RedJoker:
                return "Joker";
            default:
                throw new ArgumentException("Invalid cardRank");
        }
    }
    
}