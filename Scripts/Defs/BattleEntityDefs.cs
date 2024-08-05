using System.Collections.Generic;

namespace XCardGame;

public static class BattleEntityDefs
{
    public static BattleEntityDef DefaultPlayerBattleEntityDef = new ()
    {
        Name = "You",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        InitDeckDef = DeckDefs.PlayerInitDeckDef,
        InitAttack = 0,
        InitDefence = 0,
        InitItemPocketSize = 3,
        InitItemRecharge = 1,
        IsPlayer = true,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 100,
        InitEnergy = 3,
        InitCredit = 100
    };

    public static BattleEntityDef GamblerPlayerBattleEntityDef = GetGamblerPlayerBattleEntityDef();
    
    private static BattleEntityDef GetGamblerPlayerBattleEntityDef()
    {
        var res = DefaultPlayerBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Gambler";
        res.PortraitPath = "res://Sprites/gambler.png";
        res.SpritePath = "res://Sprites/gambler.png";
        res.InitDeckDef = new List<CardDef> { CardDefs.D6, CardDefs.LesserD6, CardDefs.LesserD6 };
        return res;
    }
    
    public static BattleEntityDef TricksterPlayerBattleEntityDef = GetTricksterPlayerBattleEntityDef();
    
    private static BattleEntityDef GetTricksterPlayerBattleEntityDef()
    {
        var res = DefaultPlayerBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Trickster";
        res.PortraitPath = "res://Sprites/trickster.png";
        res.SpritePath = "res://Sprites/trickster.png";
        res.InitDeckDef = new List<CardDef> { CardDefs.MagicalHat, CardDefs.LesserMagicalHat, CardDefs.LesserMagicalHat };
        return res;
    }

    public static BattleEntityDef PiratePlayerBattleEntityDef = GetPiratePlayerBattleEntityDef();

    private static BattleEntityDef GetPiratePlayerBattleEntityDef()
    {
        var res = DefaultPlayerBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Pirate";
        res.PortraitPath = "res://Sprites/pirate.png";
        res.SpritePath = "res://Sprites/pirate.png";
        res.InitDeckDef = new List<CardDef> { CardDefs.CopyPaste, CardDefs.LesserCopyPaste, CardDefs.LesserCopyPaste };
        return res;
    }
    
    public static BattleEntityDef ClownPlayerBattleEntityDef = GetClownPlayerBattleEntityDef(); 
        
    private static BattleEntityDef GetClownPlayerBattleEntityDef()
    {
        var res = DefaultPlayerBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Clown";
        res.PortraitPath = "res://Sprites/clown.png";
        res.SpritePath = "res://Sprites/clown.png";
        res.InitDeckDef = new List<CardDef> { CardDefs.BalaTroll };
        return res;
    }
    
    public static BattleEntityDef DefaultEnemyBattleEntityDef = new ()
    {
        Name = "Enemy",
        PortraitPath = "res://Sprites/enemy.png",
        SpritePath = "res://Sprites/enemy.png",
        InitDeckDef = null,
        InitAttack = 0,
        InitDefence = 0,
        InitHandPowers = HandPowerTables.DefaultEnemyHandPowerTable,
        InitHp = 100,
    };

    public static BattleEntityDef TestEnemyBattleEntityDef = GetTestEnemyBattleEntityDef();

    private static BattleEntityDef GetTestEnemyBattleEntityDef()
    {
        var res = DefaultEnemyBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Test";
        res.PortraitPath = "res://Sprites/test.png";
        res.SpritePath = "res://Sprites/test.png";
        res.InitHp = 1;
        return res;
    }

    public static BattleEntityDef TallBoyBattleEntityDef = GetTallBoyBattleEntityDef();

    private static BattleEntityDef GetTallBoyBattleEntityDef()
    {
        var res = DefaultEnemyBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Tall boy";
        res.PortraitPath = "res://Sprites/tall_boy.png";
        res.SpritePath = "res://Sprites/tall_boy.png";
        res.InitHandPowers = HandPowerTables.HighCardEnhancedHandPowerTable;
        return res;
    }

    public static BattleEntityDef NinjaBattleEntityDef = GetNinjaBattleEntityDef();

    private static BattleEntityDef GetNinjaBattleEntityDef()
    {
        var res = DefaultEnemyBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Ninja";
        res.PortraitPath = "res://Sprites/ninja.png";
        res.SpritePath = "res://Sprites/ninja.png";
        res.InitDeckDef = null;
        res.InitHandPowers = HandPowerTables.NoaKEnhancedHandPowerTable;
        return res;
    }

    public static BattleEntityDef ManInBlackBattleEntityDef = GetManInBlackBattleEntityDef();

    private static BattleEntityDef GetManInBlackBattleEntityDef()
    {
        var res = DefaultEnemyBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Man in black";
        res.PortraitPath = "res://Sprites/man_in_black.png";
        res.SpritePath = "res://Sprites/man_in_black.png";
        res.InitDeckDef = new List<CardDef> { CardDefs.SpadesRule, CardDefs.ClubsRule };
        res.InitHandPowers = HandPowerTables.StraightFlushEnhancedHandPowerTable;
        return res;
    }

    public static BattleEntityDef AssassinBattleEntityDef = GetAssassinBattleEntityDef();
    private static BattleEntityDef GetAssassinBattleEntityDef()
    {
        var res = DefaultEnemyBattleEntityDef.Clone<BattleEntityDef>();
        res.Name = "Assassin";
        res.PortraitPath = "res://Sprites/assassin.png";
        res.SpritePath = "res://Sprites/assassin.png";
        res.InitAttack = 20;
        return res;
    }
}