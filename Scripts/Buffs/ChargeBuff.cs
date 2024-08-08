using XCardGame.Common;

namespace XCardGame;

public class ChargeBuff: BaseBuff
{
    public ChargeBuff(int stack) :
        base(Utils._("Charge"), Utils._($"Add multiplier to the next attack by {Configuration.ChargeMultiplierPerStack} percentage per stack. Consume all stacks on effect"), 
            "res://Sprites/BuffIcons/charge.png", true, stack:stack, maxStack:Configuration.ChargeBuffMaxStack, isTemporary:true)
    {
    }
}