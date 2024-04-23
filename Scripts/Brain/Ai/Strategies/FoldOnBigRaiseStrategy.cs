using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class FoldOnBigRaiseStrategy: BaseStrategy
{
    public float AgainstPotPercentage;
    public float AgainstStackPercentage;
    public float FoldMultiplierOnBigRaiseOverPot;
    public float FoldMultiplierOnBigRaiseOverStack;
    
    public FoldOnBigRaiseStrategy(ProbabilityActionAi ai, int weightBaseline, float againstPotPercentage = 1.0f,
        float againstStackPercentage = 0.5f, float foldMultiplierOnBigRaiseOverPot = 1.0f,
        float foldMultiplierOnBigRaiseOverStack = 1.0f): base(ai, weightBaseline)
    {
        AgainstPotPercentage = againstPotPercentage;
        AgainstStackPercentage = againstStackPercentage;
        FoldMultiplierOnBigRaiseOverPot = foldMultiplierOnBigRaiseOverPot;
        FoldMultiplierOnBigRaiseOverStack = foldMultiplierOnBigRaiseOverStack;
    }
    
    public override void Trigger()
    {
        if (Ai.Hand.RoundCallAmount > Ai.Hand.Pot.Total * AgainstPotPercentage)
        {
            Ai.FoldWeight += (int)(WeightBaseline * FoldMultiplierOnBigRaiseOverPot);
        }
        if (Ai.Hand.RoundCallAmount > Ai.Player.NChipsInHand.Value * AgainstStackPercentage)
        {
            Ai.FoldWeight += (int)(WeightBaseline * FoldMultiplierOnBigRaiseOverStack);
        }
    }
}