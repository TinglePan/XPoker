using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class BuffAgainstEntityEffect: BaseAgainstEntityEffect
{

    public bool TargetSelf;
    public BaseBuff Buff;
    
    public BuffAgainstEntityEffect(BaseCard originateCard, BaseBuff buff, bool targetSelf = false): base("Buff", "Inflict {} on {}", originateCard)
    {
        TargetSelf = targetSelf;
        Buff = buff;
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Buff.Setup(args);
    }


    public override void Resolve()
    {
        var target = TargetSelf ? Self : Opponent;
        Battle.InflictBuffOn(Buff, target, Self, OriginateCard);
    }

    public override string Description()
    {
        return string.Format(DescriptionTemplate, Buff, Utils.GetPersonalPronoun(Self, Self, Opponent));
    }
}