using System.Collections.Generic;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class BaseStrategy
{
    public ProbabilityActionAi Ai;
    
    public BaseStrategy(ProbabilityActionAi ai)
    {
        Ai = ai;
    }

    public virtual bool CanTrigger()
    {
        return true;
    }
    
    public virtual void Trigger()
    {
        
    }
}