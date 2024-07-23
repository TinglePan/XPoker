using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Game;

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
    
    public int Resolve()
    {
        var attackerDamageModifier = Attacker.GetAttackerDamageModifier();
        var attackerDamageMultipliers = Attacker.GetAttackerDamageMultipliers();
        
        float attackValue = RawAttackValue + attackerDamageModifier;
        var separatedAttackerDamageMultipliers = Utils.AddUpSeparatedMultipliers(attackerDamageMultipliers);
        foreach (var attackMultiplier in attackerDamageMultipliers)
        {
            attackValue *= attackMultiplier;
        }
        var roundedAttackValue = (int)(attackValue * separatedAttackerDamageMultipliers.X * separatedAttackerDamageMultipliers.Y);

        // Blockers
        foreach (var buff in Defender.BuffContainer.Buffs)
        {
            if (buff is InvincibleBuff)
            {
                return 0;
            } else if (buff is BlockBuff blockBuff)
            {
                if (Power <= blockBuff.Stack.Value)
                {
                    return 0;
                }
            } else if (buff is EvadeBuff evadeBuff)
            {
                evadeBuff.Consume();
                return 0;
            }
        }
        
        float damageValue = roundedAttackValue;
        var defenderDamageModifier = Defender.GetDefenderDamageModifier();
        var defenderDamageMultipliers = Defender.GetDefenderDamageMultipliers();
        damageValue += defenderDamageModifier;
        
        var separatedDefenderDamageMultipliers = Utils.AddUpSeparatedMultipliers(defenderDamageMultipliers);
        var roundedDamageValue = (int)(damageValue * separatedDefenderDamageMultipliers.X * separatedDefenderDamageMultipliers.Y);
        
        Battle.GameMgr.BattleLog.Log(Utils._($"{roundedDamageValue} damage to {Defender}"));
        
        Defender.TakeDamage(roundedDamageValue);
        return roundedDamageValue;
    }
}