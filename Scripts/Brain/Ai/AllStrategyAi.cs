using System.Collections.Generic;
using XCardGame.Scripts.Brain.Ai.Strategies;

namespace XCardGame.Scripts.Brain.Ai;

public partial class AllStrategyAi: ProbabilityActionAi
{
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        FoldWeight = 0;
        BaseFoldWeight = 0;
        RaiseWeight = 0;
        BaseRaiseWeight = 0;
        Strategies.Add(new DrawingStrategy(this, 1000));
        Strategies.Add(new FoldOnBigRaiseStrategy(this, 500));
        Strategies.Add(new PositionStrategy(this, 200));
        Strategies.Add(new PreFlopStrategy(this, 1000));
        Strategies.Add(new SprStrategy(this, 200));
    }
}