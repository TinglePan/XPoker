using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public static class LevelUpTables
{
    
    public static List<BaseCard> NoAbilityCards = new();
    public static List<PokerCard> NoPokerCards = new();
    
    public static Dictionary<int, LevelUpInfo> DefaultPlayerLevelUpTable = new()
    {
        { 1, new LevelUpInfo(0, 1, 10, 0, 0, NoAbilityCards, NoPokerCards) },
        { 2, new LevelUpInfo(1, 0, 0, 0, 0, NoAbilityCards, NoPokerCards) },
        { 3, new LevelUpInfo(0, 0, 10, 0, 1, NoAbilityCards, NoPokerCards) },
        { 4, new LevelUpInfo(1, 1, 0, 0, 0, NoAbilityCards, NoPokerCards) },
        { 5, new LevelUpInfo(0, 0, 10, 1, 0, NoAbilityCards, NoPokerCards) },
        { 6, new LevelUpInfo(1, 0, 0, 0, 1, NoAbilityCards, NoPokerCards) },
        { 7, new LevelUpInfo(0, 1, 10, 1, 0, NoAbilityCards, NoPokerCards) },
    };
}