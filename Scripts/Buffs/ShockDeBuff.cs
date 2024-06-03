using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class ShockDeBuff: BaseStackableBuff
{
    public ShockDeBuff(int stack, int maxStack, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard) : 
        base("Shock", $"Each stack grants {Utils.GetPercentageString(Configuration.ShockDecreaseDamageDealtMultiplierPerStack)} less damage dealt and {Utils.GetPercentageString(Configuration.ShockIncreaseDamageReceivedMultiplierPerStack)} more damage received.",
        "res://Sprites/Icons/shock.png", stack, maxStack, entity, inflictedBy, inflictedByCard)
    {
        Battle.BeforeApplyAttack += OnBeforeApplyAttack;
    }

    protected override int StackDecreasePerRound()
    {
        return Stack.Value > 1 ? Stack.Value / 2 : 1;
    }

    private void OnBeforeApplyAttack(Battle battle, Attack attack)
    {
        if (attack.Target == Entity)
        {
            attack.ExtraMultipliers.Add((Configuration.ShockIncreaseDamageReceivedMultiplierPerStack * Stack.Value, Name));
        } else if (attack.Source == Entity)
        {
            attack.ExtraMultipliers.Add((Configuration.ShockDecreaseDamageDealtMultiplierPerStack * Stack.Value, Name));
        }
    }
    
    

}