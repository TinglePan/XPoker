using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;


namespace XCardGame.Scripts.Buffs;

public class ChargeBuff: BaseBuff
{
    public ChargeBuff(int stack) :
        base(Utils._("Charge"), Utils._("Double the attack value of following attack. Consume on effect"), "res://Sprites/BuffIcons/charge.png", 
            true, stack:stack, maxStack:Configuration.PowerBasedBuffMaxStack, isTemporary:true)
    {
    }
}