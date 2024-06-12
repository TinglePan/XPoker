using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class FeebleDeBuff: BaseBuff
{
    public FeebleDeBuff(int stack) : 
        base("Feeble", $"Reduce damage dealt 1 per stack", "res://Sprites/BuffIcons/feeble.png", 
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }
    
    public override void Consume()
    {
        ChangeStack(-1);
    }
}