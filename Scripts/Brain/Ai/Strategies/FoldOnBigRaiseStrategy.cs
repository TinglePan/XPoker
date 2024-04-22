using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class FoldOnBigRaiseStrategy: BaseStrategy
{
    public float AgainstPotPercentage;
    public float AgainstStackPercentage;
    public float FoldModifierOnBigRaiseOverPot;
    public float FoldModifierOnBigRaiseOverStack;
    
    public FoldOnBigRaiseStrategy(ProbabilityActionAi ai, float againstPotPercentage = 1.0f, float againstStackPercentage = 0.5f,
        float foldModifierOnBigRaiseOverPot = 3.0f, float foldModifierOnBigRaiseOverStack = 2.0f): base(ai)
    {
        AgainstPotPercentage = againstPotPercentage;
        AgainstStackPercentage = againstStackPercentage;
        FoldModifierOnBigRaiseOverPot = foldModifierOnBigRaiseOverPot;
        FoldModifierOnBigRaiseOverStack = foldModifierOnBigRaiseOverStack;
    }
    
    public override void Trigger()
    {
        if (Ai.Hand.RoundCallAmount > Ai.Hand.Pot.Total * AgainstPotPercentage)
        {
            Ai.FoldWeight = (int)(Ai.FoldWeight * FoldModifierOnBigRaiseOverPot);
        }
        if (Ai.Hand.RoundCallAmount > Ai.Player.NChipsInHand.Value * AgainstStackPercentage)
        {
            Ai.FoldWeight = (int)(Ai.FoldWeight * FoldModifierOnBigRaiseOverStack);
        }
    }
}