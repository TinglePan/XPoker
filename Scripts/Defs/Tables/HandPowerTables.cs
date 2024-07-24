using System.Collections.Generic;
using XCardGame.Common;

namespace XCardGame;

public static class HandPowerTables
{

    public static Dictionary<Enums.HandTier, int> EmptyHandPowerTable()
    {
        return new Dictionary<Enums.HandTier, int>()
        {
            { Enums.HandTier.HighCard, 0 },

            { Enums.HandTier.Pair, 0 },

            { Enums.HandTier.TwoPairs, 0 },

            { Enums.HandTier.ThreeOfAKind, 0 },

            { Enums.HandTier.Straight, 0 },

            { Enums.HandTier.Flush, 0 },

            { Enums.HandTier.FullHouse, 0 },

            { Enums.HandTier.Quads, 0 },

            { Enums.HandTier.StraightFlush, 0 },

            { Enums.HandTier.RoyalFlush, 0 }
        };
    }

    public static Dictionary<Enums.HandTier, int> DefaultHandPowerTable()
    {
        return new()
        {
            { Enums.HandTier.HighCard, 0 },

            { Enums.HandTier.Pair, 0 },

            { Enums.HandTier.TwoPairs, 2 },

            { Enums.HandTier.ThreeOfAKind, 4 },

            { Enums.HandTier.Straight, 6 },

            { Enums.HandTier.Flush, 6 },

            { Enums.HandTier.FullHouse, 8 },

            { Enums.HandTier.Quads, 10 },

            { Enums.HandTier.StraightFlush, 10 },

            { Enums.HandTier.RoyalFlush, 16 }
        };
    }

    public static Dictionary<Enums.HandTier, int> HighCardEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 12 },

        { Enums.HandTier.Pair, 0 },

        { Enums.HandTier.TwoPairs, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.Quads, 0 },

        { Enums.HandTier.StraightFlush, 0 },

        { Enums.HandTier.RoyalFlush, 0 }
    };
    
    public static Dictionary<Enums.HandTier, int> NoaKEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 0 },

        { Enums.HandTier.Pair, 4 },

        { Enums.HandTier.TwoPairs, 0 },

        { Enums.HandTier.ThreeOfAKind, 8 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 12 },

        { Enums.HandTier.Quads, 16 },

        { Enums.HandTier.StraightFlush, 0 },

        { Enums.HandTier.RoyalFlush, 0 }
    };
    
    public static Dictionary<Enums.HandTier, int> StraightFlushEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 0 },

        { Enums.HandTier.Pair, 0 },

        { Enums.HandTier.TwoPairs, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 12 },

        { Enums.HandTier.Flush, 12 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.Quads, 0 },

        { Enums.HandTier.StraightFlush, 18 },

        { Enums.HandTier.RoyalFlush, 0 }
    };
    
    public static Dictionary<Enums.HandTier, int> DefaultPlayerHandPowerTable = DefaultHandPowerTable();

    public static Dictionary<Enums.HandTier, int> DefaultEnemyHandPowerTable = DefaultHandPowerTable();

}