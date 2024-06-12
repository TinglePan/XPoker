using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class DamageSkillEffect: BaseSkillEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public DamageSkillEffect(BaseCard createdByCard, int rawValue,
        float powerScale) : base("Deal damage", $"Deal {rawValue} damage, every 1 power deals {powerScale} more damage", createdByCard)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override void Resolve(SkillResolver resolver, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
        var power = self.GetPower(hand.Tier);
        var rawAttackValue = CalculateValue(power);
        var attack = new Attack(self, opponent, rawAttackValue);
        attack.Resolve();
    }
    
    public int CalculateValue(int power)
    {
        return RawValue + (int)(power * PowerScale);
    }

}