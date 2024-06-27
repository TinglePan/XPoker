using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class DefendAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public DefendAgainstEntityEffect(BaseCard originateCard, int rawValue, float powerScale) : 
        base("Grant defence", $"Gain {rawValue} defence, plus {{}} * power", originateCard)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override void Resolve()
    {
        float defenceValue = RawValue;
        if (PowerScale > 0)
        {
            var power = Self.GetPower(Engage.Hands[Self].Tier);
            defenceValue = CalculateValue(power);
        }
        defenceValue += Self.GetDefenceModifier();
        var separatedMultipliers = Utils.AddUpSeparatedMultipliers(Self.GetDefenceMultipliers());
        var roundedDefenceValue = (int)(defenceValue * separatedMultipliers.X * separatedMultipliers.Y);
        Self.ChangeDefence(roundedDefenceValue);
    }
    
    public int CalculateValue(int power)
    {
        return RawValue + (int)(power * PowerScale);
    }
}