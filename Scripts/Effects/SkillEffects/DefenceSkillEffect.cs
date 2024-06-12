using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class DefenceSkillEffect: BaseSkillEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public DefenceSkillEffect(BaseCard createdByCard, int rawValue,
        float powerScale) : base("Grant defence", $"Grant {rawValue} defence, every 1 power Grants {powerScale} more defence", createdByCard)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override void Resolve(SkillResolver resolver, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
        var power = self.GetPower(hand.Tier);
        float defenceValue = CalculateValue(power);
        defenceValue += GetDefenceModifier(self);
        foreach (var defenceMultiplier in GetDefenceMultipliers(self))
        {
            defenceValue *= defenceMultiplier;
        }
        var roundedDefenceValue = (int)defenceValue;
        self.ChangeDefence(roundedDefenceValue);
    }
    
    public int CalculateValue(int power)
    {
        return RawValue + (int)(power * PowerScale);
    }

    protected int GetDefenceModifier(BattleEntity self)
    {
        var res = 0;
        foreach (var buff in self.Buffs)
        {
        }
        return res;
    }
    
    protected List<float> GetDefenceMultipliers(BattleEntity self)
    {
        List<float> res = new();
        foreach (var buff in self.Buffs)
        {
        }
        return res;
    }
}