using System.Collections.Generic;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class BaseStrategy
{
    public ProbabilityActionAi Ai;
    public int WeightBaseline;
    
    public BaseStrategy(ProbabilityActionAi ai, int weightBaseline)
    {
        Ai = ai;
        WeightBaseline = weightBaseline;
    }

    public virtual bool CanTrigger()
    {
        return true;
    }
    
    public virtual void Trigger()
    {
        
    }
}