using XCardGame.Common;

namespace XCardGame;

public class TauntedBuff: BaseBuff
{
    public TauntedBuff(int stack = 0) : base(Utils._("Courage"), Utils._($"Add {Configuration.ChargeMultiplierPerStack} percentage attack multiplier per stack."), 
        "res://Sprites/BuffIcons/courage.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}