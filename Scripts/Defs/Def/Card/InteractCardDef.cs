using System;

namespace XCardGame;

[Serializable]
public class InteractCardDef: BaseCardDef
{
    public bool IsInnate;
    public bool IsExhaust;
    public int Cost;
}