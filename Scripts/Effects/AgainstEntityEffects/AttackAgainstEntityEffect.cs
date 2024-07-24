using XCardGame.Common;

namespace XCardGame;

public class AttackAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; set; }
    public float PowerScale { get; }
    
    public float Leech { get; set; }
    
    public AttackAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale, float leech = 0f) : 
        base(Utils._("Attack"), Utils._($"Deal {rawValue} base damage, plus {{}} * attack"), originateCard, src, dst)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
        Leech = leech;
    }

    public override void Resolve()
    {
        var rawAttackValue = RawValue;
        int power = 0;
        if (PowerScale > 0)
        {
            power = Src.GetPower(Battle.RoundHands[Src].Tier, Enums.EngageRole.Attacker);
            rawAttackValue = CalculateValue(power);
        }
        
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} attacks! Atk:{power}. Base:{rawAttackValue}"));
        var attack = new Attack(Battle, Src, Dst, power, rawAttackValue);
        var damage = attack.Resolve();
        if (Leech > 0)
        {
            var leechValue = (int)(damage * Leech);
            Src.ChangeHp(leechValue);
            Battle.GameMgr.BattleLog.Log(Utils._($"{Src} leeches {leechValue} HP!"));
        }
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