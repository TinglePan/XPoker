using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class DefendAgainstEntityEffect: BaseAgainstEntityEffect, IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    
    public DefendAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int rawValue, float powerScale) : 
        base("Grant defence", $"Gain {rawValue} defence, plus {{}} * power", src, dst, originateCard)
    {
        RawValue = rawValue;
        PowerScale = powerScale;
    }
    
    public override void Resolve()
    {
        float defenceValue = RawValue;
        if (PowerScale > 0)
        {
            var power = Src.GetPower(Engage.Hands[Src].Tier);
            defenceValue = CalculateValue(power);
        }
        defenceValue += Src.GetDefenceModifier();
        var separatedMultipliers = Utils.AddUpSeparatedMultipliers(Src.GetDefenceMultipliers());
        var roundedDefenceValue = (int)(defenceValue * separatedMultipliers.X * separatedMultipliers.Y);
        Src.ChangeDefence(roundedDefenceValue);
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