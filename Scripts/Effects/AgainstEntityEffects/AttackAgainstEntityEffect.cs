using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

public class AttackAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; set; }
    public float PowerScale { get; }
    
    public AttackAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale, float leech = 0f) : 
        base(Utils._("Attack"), Utils._($"Deal {rawValue} base damage, plus {{}} * attack"), originateCard, src, dst)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }

    public override Task Apply()
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
        attack.Apply();
        return Task.CompletedTask;
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