using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Defs.Def.Deck;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Defs.Def;

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