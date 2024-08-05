using System;
using System.Collections.Generic;
using XCardGame.Common;

namespace XCardGame;

[Serializable]
public class BattleEntityDef: BaseDef
{
    public string Name;
    public string PortraitPath;
    public string SpritePath;
    public List<CardDef> InitDeckDef;
    public int InitAttack;
    public int InitDefence;
    public Dictionary<Enums.HandTier, int> InitHandPowers;
    public int InitHp;
    public int InitLevel;
    
    public bool IsPlayer;
    
    // Player
    public int InitEnergy;
    public int InitCredit;
    public int InitItemPocketSize;
    public int InitItemRecharge;

    public BattleEntityDef()
    {
        InitDeckDef = new List<CardDef>();
        InitHandPowers = new Dictionary<Enums.HandTier, int>();
    }
}