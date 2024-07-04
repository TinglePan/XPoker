using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;


namespace XCardGame.Scripts.Buffs;

public class ResistBuff: BaseBuff
{
    public ResistBuff(int stack) : 
        base("Resist", "Reduce incoming damage by 1 per stack", "res://Sprites/BuffIcons/resist.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}