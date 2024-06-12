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

    public BuffSkillEffect(BattleEntity target, BaseCard createdByCard, BaseBuff passInBuff = null) : base("Inflict", null, createdByCard)
    {
        Buff = passInBuff;
        if (Buff != null)
        {
            Description = $"Inflict {Buff}";
        }
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
}