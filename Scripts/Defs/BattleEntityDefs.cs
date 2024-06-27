using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public static class BattleEntityDefs
{
    public static PlayerBattleEntityDef DefaultPlayerBattleEntityDef = new ()
    {
        Name = "You",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        FactionId = Enums.FactionId.Player,
        InitDeckDef = DeckDefs.PlayerInitDeckDef,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 10,
        InitLevel = 1,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static BattleEntityDef DefaultEnemyBattleEntityDef = new ()
    {
        Name = "Cpu",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        FactionId = Enums.FactionId.Player,
        InitDeckDef = DeckDefs.EnemyInitDeckDef,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 1,
        InitLevel = 1,
    };
}