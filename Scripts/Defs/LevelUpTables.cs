using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public static class LevelUpTables
{
    
    public static List<BaseCard> NoCard = new();
    
    public static Dictionary<int, LevelUpInfo> DefaultPlayerLevelUpTable = new()
    {
        { 1, new LevelUpInfo(0, 10, NoCard) },
        { 2, new LevelUpInfo(1, 0, NoCard) },
        { 3, new LevelUpInfo(0, 10, NoCard) },
        { 4, new LevelUpInfo(1, 0, NoCard) },
        { 5, new LevelUpInfo(0, 10, NoCard) },
        { 6, new LevelUpInfo(1, 0, NoCard) },
        { 7, new LevelUpInfo(0, 10, NoCard) },
    };
}