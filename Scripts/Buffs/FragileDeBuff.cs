using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class FragileDeBuff: BaseBuff
{
    public FragileDeBuff(int stack) : 
        base("Fragile", "Receives 1 more damage per stack from incoming attack", "res://Sprites/BuffIcons/fragile.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}