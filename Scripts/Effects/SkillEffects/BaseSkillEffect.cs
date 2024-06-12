using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class BaseSkillEffect: BaseEffect
{
    
    public BaseSkillEffect(string name, string description, BaseCard createdByCard) : 
        base(name, description, createdByCard)
    {
    }

    public virtual void Resolve(SkillResolver resolver, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
    }
    
    
}