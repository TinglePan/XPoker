using XCardGame.Common;

namespace XCardGame;

public class EvadeBuff: BaseBuff
{
    public EvadeBuff(int stack) : base(
        Utils._("Evade"), Utils._("Negate incoming attack, consume 1 stack on effect"),
        "res://Sprites/BuffIcons/invincible.png", true, stackOnRepeat: false, stack:stack, 
        maxStack: Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}