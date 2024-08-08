using XCardGame.Common;

namespace XCardGame;

public class TempAtkBuff: BaseBuff
{
    public TempAtkBuff(int stack) :
        base(Utils._("Temporary attack"), Utils._("This turn, add attack value by 1 per stack."), "res://Sprites/BuffIcons/tmp_atk.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}