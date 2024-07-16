using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;

namespace XCardGame.Scripts.Defs.Def.BattleEntity;

public class BattleEntityDef
{
    public string Name;
    public string PortraitPath;
    public string SpritePath;
    public List<BaseCardDef> InitDeckDef;
    public int InitBaseHandPower;
    public Dictionary<Enums.HandTier, int> InitHandPowers;
    public int InitHp;
    public int InitLevel;

    public BattleEntityDef()
    {
    }
}