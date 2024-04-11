using System;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards;

public static class CardExtensions
{
    public static string ToFriendlyString(this Enums.CardSuit cardCardSuit)
    {
        switch (cardCardSuit)
        {
            case Enums.CardSuit.Clubs:
                return "\u2663"; // ♣
            case Enums.CardSuit.Diamonds:
                return "\u2666"; // ♦
            case Enums.CardSuit.Hearts:
                return "\u2665"; // ♥
            case Enums.CardSuit.Spades:
                return "\u2660"; // ♠
            default:
                return "\u0000"; // �
        }
    }
    
    public static string ToFriendlyString(this Enums.CardRank cardCardRank)
    {
        switch (cardCardRank)
        {
            case Enums.CardRank.Two:
                return "2";
            case Enums.CardRank.Three:
                return "3";
            case Enums.CardRank.Four:
                return "4";
            case Enums.CardRank.Five:
                return "5";
            case Enums.CardRank.Six:
                return "6";
            case Enums.CardRank.Seven:
                return "7";
            case Enums.CardRank.Eight:
                return "8";
            case Enums.CardRank.Nine:
                return "9";
            case Enums.CardRank.Ten:
                return "10";
            case Enums.CardRank.Jack:
                return "J";
            case Enums.CardRank.Queen:
                return "Q";
            case Enums.CardRank.King:
                return "K";
            case Enums.CardRank.Ace:
                return "A";
            case Enums.CardRank.BlackJoker:
            case Enums.CardRank.RedJoker:
                return "Joker";
            default:
                throw new ArgumentException("Invalid cardRank");
        }
    }
    
}