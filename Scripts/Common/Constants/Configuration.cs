using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Common.Constants;

public static class Configuration
{
    public static readonly int InitialHoleCardCount = 2;
    public static readonly int InitialBigBlindAmount = 10;
    public static readonly int RaiseLimit = 0;
    public static readonly Dictionary<int, int> DealCommunityCardCount = new ()
    {
        { 0, 0 },
        { 1, 3 },
        { 2, 1 },
        { 3, 1 },
    };
    public static readonly int RiverRoundIndex = 3;

}