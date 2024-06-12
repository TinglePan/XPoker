using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class PermanentPowerBuff: BaseBuff
{
    public PermanentPowerBuff(int stack) :
        base("Charge: Power", $"Grants 1 extra power per stack.", "res://Sprites/BuffIcons/permanent_power.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }
}