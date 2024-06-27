using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class AttackAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public AttackAgainstEntityEffect(BaseCard originateCard, int rawValue, float powerScale) : 
        base("Damage", $"Deal {rawValue} base damage to {{}}, plus {{}} * power", originateCard)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }

    public override void Resolve()
    {
        var rawAttackValue = RawValue;
        int power = 0;
        if (PowerScale > 0)
        {
            power = Self.GetPower(Engage.Hands[Self].Tier);
            rawAttackValue = CalculateValue(power);
        }
        var attack = new Attack(Battle, Self, Opponent, power, rawAttackValue);
        attack.Resolve();
    }
    
    public int CalculateValue(int power)
    {
        return RawValue + (int)(power * PowerScale);
    }

}