using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.BattleEntity;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Defs.Tables;


namespace XCardGame.Scripts.Defs;

public static class BattleEntityDefs
{
    public static PlayerBattleEntityDef DefaultPlayerBattleEntityDef = new ()
    {
        Name = "You",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static PlayerBattleEntityDef GamblerPlayerBattleEntityDef = new()
    {
        Name = "You",
        PortraitPath = "res://Sprites/gambler.png",
        SpritePath = "res://Sprites/gambler.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.D6 },
        InitBaseHandPower = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 100
    };
    
    public static PlayerBattleEntityDef TricksterPlayerBattleEntityDef = new()
    {
        Name = "You",
        PortraitPath = "res://Sprites/trickster.png",
        SpritePath = "res://Sprites/trickster.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.MagicalHat },
        InitBaseHandPower = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static PlayerBattleEntityDef PiratePlayerBattleEntityDef = new()
    {
        Name = "You",
        PortraitPath = "res://Sprites/trickster.png",
        SpritePath = "res://Sprites/trickster.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.CopyPaste },
        InitBaseHandPower = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };
    
    public static PlayerBattleEntityDef ClownPlayerBattleEntityDef = new()
    {
        Name = "You",
        PortraitPath = "res://Sprites/clown.png",
        SpritePath = "res://Sprites/clown.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.BalaTrollHand },
        InitBaseHandPower = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static BattleEntityDef TestEnemyBattleEntityDef = new()
    {
        Name = "Test",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 1,
    };
    
    public static BattleEntityDef DefaultEnemyBattleEntityDef = new ()
    {
        Name = "Enemy",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef TallBoyBattleEntityDef = new()
    {
        Name = "Tall boy",
        PortraitPath = "res://Sprites/tall_boy.png",
        SpritePath = "res://Sprites/tall_boy.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.HighCardEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef NinjaBattleEntityDef = new()
    {
        Name = "Ninja",
        PortraitPath = "res://Sprites/ninja.png",
        SpritePath = "res://Sprites/ninja.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.NoaKEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef ManInBlackBattleEntityDef = new()
    {
        Name = "Man in black",
        PortraitPath = "res://Sprites/man_in_black.png",
        SpritePath = "res://Sprites/man_in_black.png",
        InitDeckDef = null,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.StraightFlushEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef AssassinBattleEntityDef = new()
    {
        Name = "Assassin",
        PortraitPath = "res://Sprites/assassin.png",
        SpritePath = "res://Sprites/assassin.png",
        InitDeckDef = null,
        InitBaseHandPower = 20,
        InitHandPowers = HandPowerTables.DefaultEnemyHandPowerTable,
        InitHp = 1,
    };
}