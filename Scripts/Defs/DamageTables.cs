using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs;

public static class DamageTables
{
    
    private static Dictionary<Enums.HandTier, int> _defaultDamageTable = new()
    {
        { Enums.HandTier.HighCard, 0 },

        { Enums.HandTier.Pair, 1 },

        { Enums.HandTier.TwoPair, 3 },

        { Enums.HandTier.ThreeOfAKind, 4 },

        { Enums.HandTier.Straight, 6 },

        { Enums.HandTier.Flush, 7 },

        { Enums.HandTier.FullHouse, 7 },

        { Enums.HandTier.FourOfAKind, 9 },

        { Enums.HandTier.StraightFlush, 12 },

        { Enums.HandTier.RoyalFlush, -1 }
    };
    
    public static Dictionary<Enums.HandTier, int> DefaultPlayerDamageTable = _defaultDamageTable;

    public static Dictionary<Enums.HandTier, int> DefaultOpponentDamageTable = _defaultDamageTable;

}