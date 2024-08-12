using XCardGame.Common;

namespace XCardGame;

public class CourageBuff: BaseBuff
{
    public CourageBuff(int stack = 0) : base(Utils._("Courage"), Utils._($"Add {Configuration.CourageMultiplierPerStack} percentage attack multiplier per stack."), 
        "res://Sprites/BuffIcons/courage.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}