using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

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

    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        Buff.Setup(args.BuffSetupArgs);
    }

    public override Task Apply()
    {
        Battle.InflictBuffOn(Buff, Dst, Src, OriginateCard);
        return Task.CompletedTask;
    }

    public override string Description()
    {
        return string.Format(DescriptionTemplate, Buff, Utils.GetPersonalPronoun(Src, Src, Dst));
    }
}