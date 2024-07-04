using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;


namespace XCardGame.Scripts.Buffs;

public class ChargeBuff: BaseBuff
{
    public ChargeBuff(int stack) :
        base("Charge", "Increase power of following skill by 1 per stack.", "res://Sprites/BuffIcons/charge_power.png", 
            true, stack:stack, maxStack:Configuration.PowerBasedBuffMaxStack, isTemporary:true)
    {
    }
}