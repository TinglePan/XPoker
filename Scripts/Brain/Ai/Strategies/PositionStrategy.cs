using System;
using Godot;

namespace XCardGame.Scripts.Brain.Ai.Strategies;

public class PositionStrategy: BaseStrategy
{
    protected static Curve CheckMultiplierCurve = GD.Load<Curve>("res://Resources/CheckMult2Position.tres");
    protected static Curve RaiseMultiplierCurve = GD.Load<Curve>("res://Resources/RaiseMult2Position.tres");
    
    public PositionStrategy(ProbabilityActionAi ai, int weightBaseline) : base(ai, weightBaseline)
    {
        
    }
    
    public override void Trigger()
    {
        var player = Ai.Player;
        if (player == null)
        {
            return;
        }
        var hand = Ai.Hand;
        var position = hand.RoundActingOrder(player.PositionIndex);
        Ai.CheckOrCallWeight += (int)(WeightBaseline * CheckMultiplierCurve.Sample((float)position / (hand.ActingPlayerCount - 1)));
        Ai.RaiseWeight += (int)(WeightBaseline * RaiseMultiplierCurve.Sample((float)position / (hand.ActingPlayerCount - 1)));
    }
}