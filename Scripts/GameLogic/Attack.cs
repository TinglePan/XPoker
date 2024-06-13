using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class Attack
{
    public Battle Battle;
    public Engage Engage;
    public BattleEntity Attacker;
    public BattleEntity Defender;
    public int Power;
    public int RawAttackValue;
    
    public Attack(Battle battle, BattleEntity attacker, BattleEntity defender, int power, int rawAttackValue)
    {
        Battle = battle;
        Engage = Battle.RoundEngage;
        Attacker = attacker;
        Defender = defender;
        Power = power;
        RawAttackValue = rawAttackValue;
    }
    
    public void Resolve()
    {
        var attackerDamageModifier = Attacker.GetAttackerDamageModifier();
        var attackerDamageMultipliers = Attacker.GetAttackerDamageMultipliers();

        float attackValue = RawAttackValue + attackerDamageModifier;
        foreach (var attackMultiplier in attackerDamageMultipliers)
        {
            attackValue *= attackMultiplier;
        }
        
        var roundedAttackValue = (int)attackValue;

        // Blockers
        foreach (var buff in Defender.Buffs)
        {
            if (buff is RiposteBuff riposteBuff)
            {
                if (Power <= riposteBuff.Stack.Value)
                {
                    var riposteCounterAttack = riposteBuff.CounterAttack();
                    riposteBuff.Consume();
                    riposteCounterAttack.Resolve();
                    return;
                }
            } else if (buff is InvincibleBuff)
            {
                return;
            } else if (buff is BlockBuff blockBuff)
            {
                if (Power <= blockBuff.Stack.Value)
                {
                    return;
                }
            } else if (buff is EvadeBuff evadeBuff)
            {
                evadeBuff.Consume();
                return;
            }
        }
        
        float damageValue = roundedAttackValue;
        var defenderDamageModifier = Defender.GetDefenderDamageModifier();
        var defendDamageMultipliers = Defender.GetDefenderDamageMultipliers();
        damageValue += defenderDamageModifier;
        foreach (var defendMultiplier in defendDamageMultipliers)
        {
            damageValue *= defendMultiplier;
        }
        var roundedDamageValue = (int)damageValue;
        
        Defender.TakeDamage(roundedDamageValue);
    }
}