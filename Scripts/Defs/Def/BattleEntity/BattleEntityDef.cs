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
    public List<BaseCardDef> InitDeckDef;
    public int InitAttack;
    public int InitDefence;
    public Dictionary<Enums.HandTier, int> InitHandPowers;
    public int InitHp;
    public int InitLevel;

    public BattleEntityDef()
    {
    }
}