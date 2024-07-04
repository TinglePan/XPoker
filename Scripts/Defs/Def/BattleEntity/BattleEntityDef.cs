using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs.Def.BattleEntity;

public class BattleEntityDef
{
    public string Name;
    public string PortraitPath;
    public string SpritePath;
    public Enums.FactionId FactionId;
    public DeckDef InitDeckDef;
    public int InitBaseHandPower;
    public Dictionary<Enums.HandTier, int> InitHandPowers;
    public int InitHp;
    public int InitLevel;

    public BattleEntityDef()
    {
        FactionId = Enums.FactionId.None;
    }
}