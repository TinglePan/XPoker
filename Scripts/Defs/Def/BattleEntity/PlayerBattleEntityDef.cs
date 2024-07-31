using System;

namespace XCardGame;

[Serializable]
public class PlayerBattleEntityDef: BattleEntityDef
{
    public int InitEnergy;
    public int InitCredit;
    public int InitItemPocketSize;
    public int InitItemRecharge;
}