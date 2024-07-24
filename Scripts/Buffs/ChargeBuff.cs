using XCardGame.Common;

namespace XCardGame;

public class ChargeBuff: BaseBuff
{
    public ChargeBuff(int stack) :
        base(Utils._("Charge"), Utils._("Double the attack value of following attack. Consume on effect"), "res://Sprites/BuffIcons/charge.png", 
            true, stack:stack, maxStack:Configuration.PowerBasedBuffMaxStack, isTemporary:true)
    {
    }
}