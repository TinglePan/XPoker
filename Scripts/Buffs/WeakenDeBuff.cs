using XCardGame.Common;

namespace XCardGame;

public class WeakenDeBuff: BaseBuff
{
    public WeakenDeBuff(int stack) : base(
        "Vulnerable", $"Reduce dealing damage by {Configuration.WeakenMultiplier} percent, consumes 1 stack on taking effect",
        "res://Sprites/BuffIcons/weaken.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}