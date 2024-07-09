using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class AttackAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public AttackAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale) : 
        base("Damage", $"Deal {rawValue} base damage, plus {{}} * power", src, dst, originateCard)
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
            power = Src.GetPower(Battle.RoundHands[Src].Tier);
            rawAttackValue = CalculateValue(power);
        }
        
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} attacks! Power:{power}. Value:{rawAttackValue}"));
        var attack = new Attack(Battle, Src, Dst, power, rawAttackValue);
        attack.Resolve();
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