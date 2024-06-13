using System.Runtime.InteropServices.ComTypes;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Buffs;

public class RiposteBuff: BaseBuff
{
    public RiposteBuff(int stack) : 
        base("Riposte", $"Negate the incoming attack with power less than {{}}, if that succeeds, make a counter attack.", "res://Sprites/BuffIcons/riposte.png",
            isStackable:true, stackOnRepeat:false, stack:stack, isTemporary:true)
    {
    }

    public override string GetDescription()
    {
        return string.Format(Description, Stack.Value);
    }

    public Attack CounterAttack()
    {
        return new Attack(Battle, Entity, Battle.GetOpponentOf(Entity), Stack.Value, GetCounterAttackValue());
    }

    protected int GetCounterAttackValue()
    {
        return Stack.Value;
    }
}