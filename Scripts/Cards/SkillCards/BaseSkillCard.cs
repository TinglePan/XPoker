using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class BaseSkillCard: MarkerCard
{
    public Dictionary<Enums.EngageRole, List<BaseSkillEffect>> Contents;
    
    public BaseSkillCard(BaseCardDef def) : 
        base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        SetUpContents(args);
    }

    public bool CanTrigger(Enums.EngageRole role, CompletedHand hand)
    {
        foreach (var content in Contents[role])
        {
            if (content.CanTrigger(hand)) return true;
        }
        return false;
    }

    protected virtual void SetUpContents(Dictionary<string, object> args)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>();
    }
}