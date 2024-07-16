using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class BuffAgainstEntityEffect: BaseAgainstEntityEffect
{
    public new class SetupArgs: BaseAgainstEntityEffect.SetupArgs
    {
        public BaseBuff.SetupArgs BuffSetupArgs;
    }
    
    public BaseBuff Buff;
    
    public BuffAgainstEntityEffect(BaseCard originateCard, BaseBuff buff, BattleEntity src, BattleEntity dst):
        base(Utils._("Buff"), Utils._("Inflict {} on {}"), originateCard, src, dst)
    {
        Buff = buff;
    }

    public void Setup(SetupArgs args)
    {
        base.Setup(args);
        Buff.Setup(args.BuffSetupArgs);
    }


    public override void Resolve()
    {
        Battle.InflictBuffOn(Buff, Dst, Src, OriginateCard);
    }

    public override string Description()
    {
        return string.Format(DescriptionTemplate, Buff, Utils.GetPersonalPronoun(Src, Src, Dst));
    }
}