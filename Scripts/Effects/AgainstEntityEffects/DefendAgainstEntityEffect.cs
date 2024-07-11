﻿using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class DefendAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; set; }
    public float PowerScale { get; }
    
    public DefendAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale) : 
        base(Utils._("Defend"), Utils._($"Gain {rawValue} defence, plus {{}} * power"), originateCard, src, dst)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override void Resolve()
    {
        float defenceValue = RawValue;
        int power = 0;
        if (PowerScale > 0)
        {
            power = Src.GetPower(Battle.RoundHands[Src].Tier);
            defenceValue = CalculateValue(power);
        }
        defenceValue += Src.GetDefenceModifier();
        var separatedMultipliers = Utils.AddUpSeparatedMultipliers(Src.GetDefenceMultipliers());
        var roundedDefenceValue = (int)(defenceValue * separatedMultipliers.X * separatedMultipliers.Y);
        Src.ChangeDefence(roundedDefenceValue);
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} defends! Power:{power}. Value:{defenceValue}"));
        Battle.GameMgr.BattleLog.Log(Utils._($"Gained {roundedDefenceValue} defence"));
    }
    
    public override string Description()
    {
        return string.Format(DescriptionTemplate, PowerScale);
    }
    
    public int CalculateValue(int power)
    {
        return RawValue + (int)(power * PowerScale);
    }
}