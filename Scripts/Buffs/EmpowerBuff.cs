using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;


namespace XCardGame.Scripts.Buffs;

public class EmpowerBuff: BaseBuff
{
    public EmpowerBuff(int stack) :
        base("Empower", $"Grants 1 extra power per stack.", "res://Sprites/BuffIcons/permanent_power.png",
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}