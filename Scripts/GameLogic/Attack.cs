using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class Attack
{
    public BattleEntity Attacker;
    public BattleEntity Defender;
    public int RawAttackValue;
    
    public Attack(BattleEntity attacker, BattleEntity defender, int rawAttackValue)
    {
        Attacker = attacker;
        Defender = defender;
        RawAttackValue = rawAttackValue;
    }
    
    public void Resolve()
    {
        var attackerDamageModifier = GetAttackerDamageModifier(Attacker);
        var attackerDamageMultipliers = GetAttackerDamageMultipliers(Attacker);

        float attackValue = RawAttackValue + attackerDamageModifier;
        foreach (var attackMultiplier in attackerDamageMultipliers)
        {
            attackValue *= attackMultiplier;
        }
        
        var roundedAttackValue = (int)attackValue;

        float damageValue = roundedAttackValue;
        var defenderDamageModifier = GetDefenderDamageModifier(Defender);
        var defendDamageMultipliers = GetDefenderDamageMultipliers(Defender);
        damageValue += defenderDamageModifier;
        foreach (var defendMultiplier in defendDamageMultipliers)
        {
            damageValue *= defendMultiplier;
        }
        var roundedDamageValue = (int)damageValue;
        Defender.TakeDamage(roundedDamageValue);
    }
    
    
    protected int GetAttackerDamageModifier(BattleEntity attacker)
    {
        var res = 0;
        foreach (var buff in attacker.Buffs)
        {
            if (buff is FeebleDeBuff)
            {
                res -= buff.Stack.Value;
            }
        }
        return res;
    }

    protected List<float> GetAttackerDamageMultipliers(BattleEntity attacker)
    {
        List<float> res = new();
        foreach (var buff in attacker.Buffs)
        {
            if (buff is WeakenDeBuff)
            {
                res.Add(1 - (float)Configuration.WeakenMultiplier / 100);
            }
        }
        return res;
    }

    protected int GetDefenderDamageModifier(BattleEntity defender)
    {
        var res = 0;
        foreach (var buff in defender.Buffs)
        {
            if (buff is FragileDeBuff)
            {
                res += buff.Stack.Value;
            }
        }
        return res;
    }

    protected List<float> GetDefenderDamageMultipliers(BattleEntity defender)
    {
        List<float> res = new();
        foreach (var buff in defender.Buffs)
        {
            if (buff is VulnerableDeBuff)
            {
                res.Add(1 + (float)Configuration.VulnerableMultiplier / 100);
            }
        }
        return res;
    }
}