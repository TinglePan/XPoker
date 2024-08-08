using XCardGame.Common;

namespace XCardGame;

public class TempAtkDeBuff: BaseBuff
{
    public TempAtkDeBuff(int stack) : 
        base(Utils._("Temporary attack cut"), Utils._("This turn, reduce attack value by 1 per stack"), "res://Sprites/BuffIcons/tmp_atk_cut.png", 
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}