using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs;

public class BattleEntityDef
{
    public string Name;
    public string PortraitPath;
    public string SpritePath;
    public Enums.FactionId FactionId;
    public DeckDef InitDeckDef;
    public int InitSpeed;
    public int InitBaseHandPower;
    public Dictionary<Enums.HandTier, int> InitHandPowers;
    public int InitHp;
    public int InitLevel;
    public List<AbilityCardDef> InitAbilityCardDefs;
    public List<SkillCardDef> InitSkillCardDefs;
}

public class PlayerBattleEntityDef: BattleEntityDef
{
    public int InitCost;
    public int InitCredit;
}

public static class BattleEntityDefs
{
    public static PlayerBattleEntityDef DefaultPlayerBattleEntityDef = new ()
    {
        Name = "You",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        FactionId = Enums.FactionId.Player,
        InitDeckDef = DeckDefs.PlayerInitDeckDef,
        InitSpeed = 10,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 10,
        InitLevel = 1,
        InitAbilityCardDefs = new List<AbilityCardDef>()
        {
            CardDefs.D6,
            CardDefs.MagicalHat,
        },
        InitSkillCardDefs = null,
        InitCost = 3,
        InitCredit = 0
    };

    public static BattleEntityDef DefaultEnemyBattleEntityDef = new ()
    {
        Name = "Cpu",
        PortraitPath = "res://Sprites/duster_guy.png",
        SpritePath = "res://Sprites/duster_guy.png",
        FactionId = Enums.FactionId.Player,
        InitDeckDef = DeckDefs.PlayerInitDeckDef,
        InitSpeed = 10,
        InitBaseHandPower = 0,
        InitHandPowers = HandPowerTables.DefaultPlayerHandPowerTable,
        InitHp = 1,
        InitLevel = 1,
        InitAbilityCardDefs = null,
        InitSkillCardDefs = null
    };
}