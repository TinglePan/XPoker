using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class RiposteBuff: BaseBuff
{
    public RiposteBuff(int power) : 
        base("Riposte", null, "res://Sprites/BuffIcons/vulnerable.png",
            isStackable:true, stackOnRepeat:false, stack:power, isTemporary:true)
    {
        Description = $"Negate the incoming attack with power less than {power}, if that succeeds, make a counter attack.";
    }
    
}