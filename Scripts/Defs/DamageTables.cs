using System.Collections.Generic;
using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs;

public static class DamageTables
{
    
    private static Dictionary<Enums.HandTier, int> _defaultDamageTable = new()
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
    
    public static Dictionary<Enums.HandTier, int> HighCardPairStrengthenedDamageTable = new()
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
    
    public static Dictionary<Enums.HandTier, int> HighCardHybridDamageTable = new()
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
    
    public static Dictionary<Enums.HandTier, int> NoaKEnhancedDamageTable = new()
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
    
    public static Dictionary<Enums.HandTier, int> StraightEnhancedDamageTable = new()
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
    
    
    public static Dictionary<Enums.HandTier, int> FlushEnhancedDamageTable = new()
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
    
    public static Dictionary<Enums.HandTier, int> DefaultPlayerDamageTable = _defaultDamageTable;

    public static Dictionary<Enums.HandTier, int> DefaultOpponentDamageTable = _defaultDamageTable;

}