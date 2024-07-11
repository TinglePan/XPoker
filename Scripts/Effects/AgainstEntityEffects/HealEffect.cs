using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

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
    
    public override void Resolve()
    {
        float healValue = RawValue;
        int power = 0;
        if (PowerScale > 0)
        {
            power = Src.GetPower(Battle.RoundHands[Src].Tier);
            healValue = CalculateValue(power);
        }
        var roundedHealValue = (int)(healValue);
        Src.ChangeHp(roundedHealValue);
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} heals for {roundedHealValue}"));
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