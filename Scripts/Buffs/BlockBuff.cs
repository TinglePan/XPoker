using XCardGame.Common;

namespace XCardGame;

public class BlockBuff: BaseBuff
{
    public BlockBuff(int stack) : base(
        Utils._("Block"), Utils._("Negate incoming attack with strength less than"),
        "res://Sprites/BuffIcons/block.png", true, stackOnRepeat: false, stack:stack, 
        maxStack:Configuration.PowerBasedBuffMaxStack, isTemporary:true)
    {
    }

    public override string Description()
    {
        return string.Format(DescriptionTemplate, Stack.Value);
    }
}