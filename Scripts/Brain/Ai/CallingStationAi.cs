using System.Collections.Generic;

namespace XCardGame.Scripts.Brain.Ai;

public partial class CallingStationAi: ProbabilityActionAi
{
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        FoldWeight = 0;
        BaseFoldWeight = 0;
        RaiseWeight = 0;
        BaseRaiseWeight = 0;
        CheckOrCallWeight = 1000;
        BaseCheckOrCallWeight = 1000;
    }
}