using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Buffs;

public class FragileDeBuff: BaseBuff
{
    public FragileDeBuff(int stack) : base(
        "Fragile", $"Reduce defence gain by {Configuration.FragileMultiplier} percent, consumes 1 stack on taking effect",
        "res://Sprites/BuffIcons/fragile.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}