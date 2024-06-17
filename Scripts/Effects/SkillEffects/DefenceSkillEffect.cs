using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class DefenceSkillEffect: BaseSkillEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public DefenceSkillEffect(Battle battle, BaseCard createdByCard, Enums.HandTier triggerHandTier, int rawValue, float powerScale) : 
        base("Grant defence", $"Grant {rawValue} defence, every 1 power Grants {powerScale} more defence", battle, createdByCard, triggerHandTier)
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
        // foreach (var buff in self.Buffs)
        // {
        // }
        return res;
    }
    
    protected List<float> GetDefenceMultipliers(BattleEntity self)
    {
        List<float> res = new();
        foreach (var buff in self.Buffs)
        {
            if (buff is FragileDeBuff)
            {
                res.Add(1 - (float)Configuration.FragileMultiplier / 100);
            }
        }
        return res;
    }
}