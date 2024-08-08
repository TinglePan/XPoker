using XCardGame.Common;

namespace XCardGame;

public class TempDefBuff: BaseBuff
{
    public TempDefBuff(int stack) :
        base(Utils._("Temporary defence"), Utils._("This turn, add defence value by 1 per stack."), "res://Sprites/BuffIcons/tmp_def.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}