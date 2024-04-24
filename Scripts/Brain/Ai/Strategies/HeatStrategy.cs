using System;
using Godot;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class HeatStrategy: BaseStrategy
{
    protected static Curve CheckAndRaiseMultiplierCurve = GD.Load<Curve>("res://Resources/CheckAndRaiseMult2Heat.tres");
    
    public HeatStrategy(ProbabilityActionAi ai, int weightBaseline) : base(ai, weightBaseline)
    {
        
    }
    
    public override bool CanTrigger()
    {
        return Ai.Player is PokerPlayerWithHeat;
    }
    
    public override void Trigger()
    {
        var player = Ai.Player as PokerPlayerWithHeat;
        if (player == null)
        {
            return;
        }
        var hand = Ai.Hand;
        var multiplier = CheckAndRaiseMultiplierCurve.Sample((float)player.Heat.Value / player.HeatThreshold.Value);
        Ai.CheckOrCallWeight += (int)(WeightBaseline * multiplier);
        Ai.RaiseWeight += (int)(WeightBaseline * multiplier);
    }
    
}