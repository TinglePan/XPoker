using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Buffs;

public class CrippleDeBuff: BaseBuff
{
    public CrippleDeBuff(int stack) : 
        base(Utils._("Cripple"), Utils._("Reduce power by 1 per stack"), "res://Sprites/BuffIcons/cripple.png", 
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}