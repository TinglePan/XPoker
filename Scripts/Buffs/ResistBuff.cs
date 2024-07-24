using XCardGame.Common;

namespace XCardGame;

public class ResistBuff: BaseBuff
{
    public ResistBuff(int stack) : 
        base("Resist", "Reduce incoming damage by 1 per stack", "res://Sprites/BuffIcons/resist.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}