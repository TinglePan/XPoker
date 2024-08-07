using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

public class HealEffect: BaseAgainstEntityEffect
{
    public int RawValue { get; set; }
    public float PowerScale { get; }
    public HealEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale) : 
        base(Utils._("Heal"), Utils._($"Heal {rawValue}, plus {{}} * power"), originateCard, src, dst)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override Task Apply()
    {
        float healValue = RawValue;
        if (PowerScale > 0)
        {
            var power = Src.GetPower(Battle.RoundHands[Src].Tier, Enums.EngageRole.Defender);
            healValue = CalculateValue(power);
        }
        var roundedHealValue = (int)(healValue);
        Src.ChangeHp(roundedHealValue);
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} heals for {roundedHealValue}"));
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