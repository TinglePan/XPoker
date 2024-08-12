using XCardGame.Common;

namespace XCardGame;

public class CautiousBuff: BaseBuff
{
    public CautiousBuff(int stack = 0) : base(Utils._("Cautious"), Utils._($"Add {Configuration.CautiousMultiplierPerStack} percentage receive damage cut per stack."), 
        "res://Sprites/BuffIcons/cautious.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}