using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class WeakenDeBuff: BaseBuff
{
    public WeakenDeBuff(int stack) : base(
        "Vulnerable", $"Reduce damage dealt by {Configuration.WeakenMultiplier} percent, consumes 1 stack on taking effect",
        "res://Sprites/BuffIcons/weaken.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}