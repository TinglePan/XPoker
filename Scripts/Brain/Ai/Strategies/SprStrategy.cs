using System;
using Godot;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class SprStrategy: BaseStrategy
{
    
    protected static Curve FoldMultiplierCurve = GD.Load<Curve>("res://Resources/FoldMult2Spr.tres");
    protected static Curve RaiseAmountMultiplierCurve = GD.Load<Curve>("res://Resources/RaiseAmountMult2Spr.tres");
    
    public SprStrategy(ProbabilityActionAi ai, int weightBaseline) : base(ai, weightBaseline)
    {
        
    }

    public override void Trigger()
    {
        var hand = Ai.Hand;
        var validStackAmount = hand.ValidStackAmount();
        var sprNormalized = (float)validStackAmount / (validStackAmount + hand.Pot.Total);
        Ai.FoldWeight += (int)(WeightBaseline * (FoldMultiplierCurve.Sample(sprNormalized) - 0.5f));
        Ai.RaiseAmountMultiplier *= 1 + (RaiseAmountMultiplierCurve.Sample(sprNormalized) - 0.5f) * 2;
    }
}