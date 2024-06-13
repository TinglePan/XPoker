using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Buffs;

public class EvadeBuff: BaseBuff
{
    public EvadeBuff(int stack) : base(
        "Evade", "Negate incoming attack, consume 1 stack on effect",
        "res://Sprites/BuffIcons/invincible.png", true, stackOnRepeat: false, stack:stack, 
        maxStack: Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}