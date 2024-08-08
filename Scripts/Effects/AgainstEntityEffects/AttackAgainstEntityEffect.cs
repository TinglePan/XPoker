using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

public class AttackAgainstEntityEffect: BaseAgainstEntityEffect
{
    public int BaseValue;
    public bool AddHandPower;
    
    public AttackAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int baseValue, bool addHandPower = true) : 
        base(Utils._("Attack"), Utils._($"Deal {baseValue} base damage"), originateCard, src, dst)
    {
        BaseValue = baseValue;
        AddHandPower = addHandPower;
    }

    public override Task Apply()
    {
        var modifiedAtk = BaseValue;
        int atkMod = Src.GetAttackModifier();
        modifiedAtk += atkMod;
        int handPower = 0;
        if (AddHandPower)
        {
            handPower = Src.HandPowers[Battle.RoundHands[Src].Tier];
            modifiedAtk += handPower;
        }
        var separatedMultipliers = Utils.AddUpSeparatedMultipliers(Src.GetAttackMultipliers());
        var res = (int)(modifiedAtk * separatedMultipliers.X * separatedMultipliers.Y);

        if (AddHandPower)
        {
            Battle.GameMgr.BattleLog.Log(Utils._($"{Src} attacks! Base:{BaseValue}, AtkModifier:{atkMod}, AtkMultipliers: {separatedMultipliers.X}/{separatedMultipliers.Y}, HandPower:{handPower}, FinalValue:{res}"));
        }
        else
        {
            Battle.GameMgr.BattleLog.Log(Utils._($"{Src} attacks! Base:{BaseValue}, AtkModifier:{atkMod}, AtkMultipliers: {separatedMultipliers.X}/{separatedMultipliers.Y}, FinalValue:{res}"));
        }
        
        // Blockers
        foreach (var buff in Dst.BuffContainer.Buffs)
        {
            if (buff is InvincibleBuff)
            {
                Battle.GameMgr.BattleLog.Log(Utils._($"But {Dst} is invincible!"));
                return Task.CompletedTask;
            }
            if (buff is BlockBuff blockBuff)
            {
                if (modifiedAtk <= blockBuff.Stack.Value)
                {
                    Battle.GameMgr.BattleLog.Log(Utils._($"{Dst} blocked({blockBuff.Stack.Value} vs {modifiedAtk})!"));
                    return Task.CompletedTask;
                }
            } else if (buff is EvadeBuff evadeBuff)
            {
                Battle.GameMgr.BattleLog.Log(Utils._($"{Dst} evades!"));
                evadeBuff.Consume();
                return Task.CompletedTask;
            }
        }
        
        var attack = new Attack(Battle, Src, Dst, res);
        attack.Apply();
        return Task.CompletedTask;
    }
}