using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class BuffSkillEffect: BaseSkillEffect
{
    public BaseBuff Buff;
    public BattleEntity Target;

    public BuffSkillEffect(BattleEntity target, BaseCard createdByCard, Enums.HandTier triggerHandTier, BaseBuff passInBuff = null, int expanding = 0) : 
        base("Inflict", "Inflict {}", createdByCard, triggerHandTier, expanding)
    {
        Buff = passInBuff;
        Target = target;
    }
    
    public override void Resolve(SkillResolver resolver, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
        Battle.InflictBuffOn(PrepareBuff(hand, self, opponent), Target, self, CreatedByCard);
    }
    
    public virtual BaseBuff PrepareBuff(CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
        return Buff;
    }

    public override string GetDescription()
    {
        return Buff == null ? string.Empty : string.Format(Description, Buff);
    }
}