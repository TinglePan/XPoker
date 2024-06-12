using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class ChargePowerBuff: BaseBuff
{
    public ChargePowerBuff(int stack) :
        base("Charge: Power", "Grants 1 extra power per stack, to the next skill used.", "res://Sprites/BuffIcons/charge_power.png", 
            true, stack:stack, isTemporary:true)
    {
    }
}