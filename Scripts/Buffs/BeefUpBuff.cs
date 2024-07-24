using XCardGame.Common;

namespace XCardGame;

public class BeefUpBuff: BaseBuff
{
    public BeefUpBuff(int stack) :
        base(Utils._("BeefUp"), Utils._("Add attack value by 1 per stack."), "res://Sprites/BuffIcons/beef_up.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}