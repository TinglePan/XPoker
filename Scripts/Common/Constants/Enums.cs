﻿namespace XCardGame.Scripts.Common.Constants;

public static class Enums
{
    public enum CardSuit
    {
        None,
        Diamonds,
        Clubs,
        Hearts,
        Spades,
        Joker,
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
        Joker = 15,
    }

    public enum CardFace
    {
        Up,
        Down
    }

    public enum Direction1D
    {
        Neutral,
        Left,
        Right
    }

    public enum Direction2D4Ways
    {
        Neutral,
        Up,
        Down,
        Left,
        Right
    }

    public enum Direction2D8Ways
    {
        Neutral,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
    
    
    public enum HandTier
    {
        HighCard,
        Pair,
        TwoPairs,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        Quads,
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
        None,
        Player,
        Enemy
    }

    public enum TapDirection
    {
        Tapped,
        UnTapped
    }

    public enum EngageRole
    {
        None,
        Attacker,
        Defender
    }

    public enum InteractCardType
    {
        None,
        Item,
        Equipment,
        Rule
    }

    public enum InteractionType
    {
        Use,
        Seal,
        Custom
    }

    public enum RuleNature
    {
        Blessing,
        Neutral,
        Curse
    }

    public enum ExhaustBehavior
    {
        Tap,
        Discard,
        Dispose
    }
}