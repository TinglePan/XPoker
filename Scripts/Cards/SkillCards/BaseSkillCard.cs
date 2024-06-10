using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class BaseSkillCard: BaseCard
{
    public Enums.HandTier TriggerHandTier;
    public int ExtraDamage;
    public float ExtraMultiplier;
    public int ExtraAttacks;
    public List<BaseBuff> BuffSelf;
    public List<BaseBuff> BuffOpponent;

    public List<BaseAttackEffect> Effects;
    
    public BaseSkillCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, 
        Enums.HandTier triggerHandTier, int extraDamage, float extraMultiplier, int extraAttacks, List<BaseBuff> buffSelf, 
        List<BaseBuff> buffOpponent, List<BaseAttackEffect> effects, BattleEntity owner) : base(name, description, iconPath, suit, rank, owner)
    {
        TriggerHandTier = triggerHandTier;
        ExtraDamage = extraDamage;
        ExtraMultiplier = extraMultiplier;
        ExtraAttacks = extraAttacks;
        BuffSelf = buffSelf;
        BuffOpponent = buffOpponent;
        Effects = effects;
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Battle.BeforeApplyAttack += BeforeApplyAttack;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        Battle.BeforeApplyAttack -= BeforeApplyAttack;
    }

    public virtual void AfterDamageDealt(Attack attack, int actualDamageDealt)
    {
        if (BuffSelf is { Count: > 0 })
        {
            foreach (var buff in BuffSelf)
            {
                Battle.InflictBuffOn(buff, attack.Source);
            }
        }
        if (BuffOpponent is { Count: > 0 })
        {
            foreach (var buff in BuffOpponent)
            {
                Battle.InflictBuffOn(buff, attack.Target);
            }
        }
        if (Effects is { Count: > 0 })
        {
            foreach (var effect in Effects)
            {
                Battle.StartEffect(effect);
            }
        }
    }

    public  void BeforeApplyAttack(Battle battle, Attack attack)
    {
        if (attack.SourceHand.Tier == TriggerHandTier)
        {
            attack.RelevantSkillCards.Add(this);
            InnerBeforeDamageDealt(attack);
        }
    }
    
    protected virtual void InnerBeforeDamageDealt(Attack attack)
    {
        if (ExtraDamage != 0)
        {
            attack.ExtraDamages.Add((ExtraDamage, Name));
        }
        if (ExtraMultiplier >= 0)
        {
            attack.ExtraMultipliers.Add(((float)ExtraMultiplier / 100, Name));
        }
    }
}