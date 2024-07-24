using System;

namespace XCardGame;

[Serializable]
public class RuleCardDef: InteractCardDef
{
    public int RankChangePerTurn;
    public bool AutoUnSeal;
}