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
        InitDeckDef = DeckDefs.PlayerInitDeckDef,
        InitAttack = 0,
        InitDefence = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static PlayerBattleEntityDef GamblerPlayerBattleEntityDef = new()
    {
        Name = "Gambler",
        PortraitPath = "res://Sprites/gambler.png",
        SpritePath = "res://Sprites/gambler.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.D6, CardDefs.LesserD6, CardDefs.LesserD6 },
        InitAttack = 0,
        InitDefence = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 1000,
        InitEnergy = 3,
        InitCredit = 100
    };
    
    public static PlayerBattleEntityDef TricksterPlayerBattleEntityDef = new()
    {
        Name = "Trickster",
        PortraitPath = "res://Sprites/trickster.png",
        SpritePath = "res://Sprites/trickster.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.MagicalHat, CardDefs.LesserMagicalHat, CardDefs.LesserMagicalHat },
        InitAttack = 0,
        InitDefence = 0,
        InitItemPocketSize = 3,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 0
    };

    public static PlayerBattleEntityDef PiratePlayerBattleEntityDef = new()
    {
        Name = "Pirate",
        PortraitPath = "res://Sprites/trickster.png",
        SpritePath = "res://Sprites/trickster.png",
        InitDeckDef = new List<BaseCardDef> { CardDefs.CopyPaste, CardDefs.LesserCopyPaste, CardDefs.LesserCopyPaste },
        InitAttack = 0,
        InitDefence = 0,
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
        InitDeckDef = new List<BaseCardDef> { CardDefs.BalaTrollHand, CardDefs.LesserBalaTrollHand, CardDefs.LesserBalaTrollHand },
        InitAttack = 0,
        InitDefence = 0,
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
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 1,
    };
    
    public static BattleEntityDef DefaultEnemyBattleEntityDef = new ()
    {
        Name = "Enemy",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        InitDeckDef = null,
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef TallBoyBattleEntityDef = new()
    {
        Name = "Tall boy",
        PortraitPath = "res://Sprites/tall_boy.png",
        SpritePath = "res://Sprites/tall_boy.png",
        InitDeckDef = null,
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.HighCardEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef NinjaBattleEntityDef = new()
    {
        Name = "Ninja",
        PortraitPath = "res://Sprites/ninja.png",
        SpritePath = "res://Sprites/ninja.png",
        InitDeckDef = null,
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.NoaKEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef ManInBlackBattleEntityDef = new()
    {
        Name = "Man in black",
        PortraitPath = "res://Sprites/man_in_black.png",
        SpritePath = "res://Sprites/man_in_black.png",
        InitDeckDef = null,
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.StraightFlushEnhancedHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef AssassinBattleEntityDef = new()
    {
        Name = "Assassin",
        PortraitPath = "res://Sprites/assassin.png",
        SpritePath = "res://Sprites/assassin.png",
        InitDeckDef = null,
        InitAttack = 20,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.DefaultEnemyHandPowerTable,
        InitHp = 1,
    };
}