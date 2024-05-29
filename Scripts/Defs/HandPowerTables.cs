using System.Collections.Generic;
using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs;

public static class HandPowerTables
{
    
    private static Dictionary<Enums.HandTier, int> _defaultHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 1 },

        { Enums.HandTier.Pair, 1 },

        { Enums.HandTier.TwoPair, 3 },

        { Enums.HandTier.ThreeOfAKind, 4 },

        { Enums.HandTier.Straight, 6 },

        { Enums.HandTier.Flush, 7 },

        { Enums.HandTier.FullHouse, 7 },

        { Enums.HandTier.FourOfAKind, 9 },

        { Enums.HandTier.StraightFlush, 12 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    public static Dictionary<Enums.HandTier, int> HighCardPairStrengthenedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 2 },

        { Enums.HandTier.Pair, 3 },

        { Enums.HandTier.TwoPair, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.FourOfAKind, 0 },

        { Enums.HandTier.StraightFlush, 0 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    public static Dictionary<Enums.HandTier, int> HighCardHybridHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 6 },

        { Enums.HandTier.Pair, 0 },

        { Enums.HandTier.TwoPair, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.FourOfAKind, 0 },

        { Enums.HandTier.StraightFlush, 0 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    public static Dictionary<Enums.HandTier, int> NoaKEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 1 },

        { Enums.HandTier.Pair, 2 },

        { Enums.HandTier.TwoPair, 0 },

        { Enums.HandTier.ThreeOfAKind, 6 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.FourOfAKind, 12 },

        { Enums.HandTier.StraightFlush, 0 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    public static Dictionary<Enums.HandTier, int> StraightEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 1 },

        { Enums.HandTier.Pair, 0 },

        { Enums.HandTier.TwoPair, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 13 },

        { Enums.HandTier.Flush, 0 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.FourOfAKind, 0 },

        { Enums.HandTier.StraightFlush, 20 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    
    public static Dictionary<Enums.HandTier, int> FlushEnhancedHandPowerTable = new()
    {
        { Enums.HandTier.HighCard, 1 },

        { Enums.HandTier.Pair, 0 },

        { Enums.HandTier.TwoPair, 0 },

        { Enums.HandTier.ThreeOfAKind, 0 },

        { Enums.HandTier.Straight, 0 },

        { Enums.HandTier.Flush, 12 },

        { Enums.HandTier.FullHouse, 0 },

        { Enums.HandTier.FourOfAKind, 0 },

        { Enums.HandTier.StraightFlush, 20 },

        { Enums.HandTier.RoyalFlush, 999 }
    };
    
    public static Dictionary<Enums.HandTier, int> DefaultPlayerHandPowerTable = _defaultHandPowerTable;

    public static Dictionary<Enums.HandTier, int> DefaultEnemyHandPowerTable = _defaultHandPowerTable;

}