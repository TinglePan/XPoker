using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class FoldOnBigRaiseStrategy: BaseStrategy
{
    public float AgainstPotPercentage;
    public float AgainstStackPercentage;
    public int FoldWeightOnBigRaiseOverPot;
    public int FoldWeightOnBigRaiseOverStack;
    
    public FoldOnBigRaiseStrategy(ProbabilityActionAi ai, float againstPotPercentage = 1.0f, float againstStackPercentage = 0.5f,
        int foldWeightOnBigRaiseOverPot = 1000, int foldWeightOnBigRaiseOverStack = 1000): base(ai)
    {
        AgainstPotPercentage = againstPotPercentage;
        AgainstStackPercentage = againstStackPercentage;
        FoldWeightOnBigRaiseOverPot = foldWeightOnBigRaiseOverPot;
        FoldWeightOnBigRaiseOverStack = foldWeightOnBigRaiseOverStack;
    }
    
    public override void Trigger()
    {
        if (Ai.Hand.RoundCallAmount > Ai.Hand.Pot.Total * AgainstPotPercentage)
        {
            Ai.FoldWeight += FoldWeightOnBigRaiseOverPot;
        }
        if (Ai.Hand.RoundCallAmount > Ai.Player.NChipsInHand.Value * AgainstStackPercentage)
        {
            Ai.FoldWeight += FoldWeightOnBigRaiseOverStack;
        }
    }
}