using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseTapCard: BaseInteractCard, ITapCard
{
    public int TappedCost { get; private set; }
    public int UnTappedCost { get; private set; }

    public int TapCostChange => TappedCost - UnTappedCost;
    
    public int UnTapCostChange => UnTappedCost - TappedCost;
    
    public BaseEffect Effect { get; protected set; }
    
    public BaseTapCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost) : base(name, description, iconPath, suit, rank)
    {
        TappedCost = tappedCost;
        UnTappedCost = unTappedCost;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }
    
    public override bool CanInteract()
    {
        return ((CardContainer)Node.Container).AllowInteract && 
               (!Node.IsTapped && Battle.Player.Cost.Value + TapCostChange <= Battle.Player.MaxCost.Value ||
                Node.IsTapped && Battle.Player.Cost.Value + UnTapCostChange <= Battle.Player.MaxCost.Value);
    }

    public void StartEffect()
    {
        if (((CardContainer)Node.Container).AllowInteract)
        {
            Battle.StartEffect(Effect);
        }
    }

    public void ToggleTap()
    {
        Node.TweenTap(!Node.IsTapped, Configuration.TapTweenTime);
        Battle.Player.Cost.Value += Node.IsTapped ? TapCostChange : UnTapCostChange;
    }

    // NOTE: Effect manages its stop on its own. And check if its source card in in position to decide whether its effect is skipped.
    // public override void OnStop(Battle battle)
    // {
    //     battle.StopEffect(Effect);
    // }
    
    protected void OnRoundStart(Battle battle)
    {
        if (!Node.IsTapped && !Node.IsNegated && Effect != null)
        {
            Battle.StartEffect(Effect);
        } 
    }
}