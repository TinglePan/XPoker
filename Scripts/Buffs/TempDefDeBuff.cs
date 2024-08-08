using XCardGame.Common;

namespace XCardGame;

public class TempDefDeBuff: BaseBuff
{
    public TempDefDeBuff(int stack) : base(
        Utils._("Temporary defence"), $"This turn, reduce defence by 1 per stack.",
        "res://Sprites/BuffIcons/tmp_def_cut.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}