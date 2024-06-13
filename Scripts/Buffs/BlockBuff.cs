using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Buffs;

public class BlockBuff: BaseBuff
{
    public BlockBuff(int stack) : base(
        "Block", "Negate incoming attack with power less than {} this round",
        "res://Sprites/BuffIcons/invincible.png", true, stackOnRepeat: false, stack:stack, 
        maxStack:Configuration.PowerBasedBuffMaxStack, isTemporary:true)
    {
    }

    public override string GetDescription()
    {
        return string.Format(Description, Stack.Value);
    }
}