using System;

namespace XCardGame;

[Serializable]
public class PiledInteractCardDef: InteractCardDef
{
    public int PileCardCountMin;
    public int PileCardCountMax;
}