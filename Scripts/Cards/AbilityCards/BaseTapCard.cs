using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseTapCard: BaseInteractCard, ITapCard
{

    public readonly TapCardDef TapCardDef;
    
    public int TappedCost => TapCardDef.TappedCost;
    public int UnTappedCost => TapCardDef.Cost;

    public int TapCostChange;
    
    public int UnTapCostChange;
    
    public BaseEffect Effect { get; protected set; }
    
    public BaseTapCard(TapCardDef def) : base(def)
    {
        TapCardDef = def;
        TapCostChange = TappedCost - UnTappedCost;
        UnTapCostChange = UnTappedCost - TappedCost;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }
    
    public override bool CanInteract()
    {
        var node = Node<CardNode>();
        if (node.Container is CardContainer { AllowInteract: false })
        {
            return false;
        }
        return !IsTapped && Battle.Player.Cost.Value + TapCostChange <= Battle.Player.MaxCost.Value ||
                IsTapped && Battle.Player.Cost.Value + UnTapCostChange <= Battle.Player.MaxCost.Value;
    }

    public void StartEffect()
    {
        var node = Node<CardNode>();
        if (node.Container is CardContainer { AllowInteract: true })
        {
            Battle.StartEffect(Effect);
        }
    }

    // NOTE: Effect manages its stop on its own. And check if its source card in in position to decide whether its effect is skipped.
    // public override void OnStop(Battle battle)
    // {
    //     battle.StopEffect(Effect);
    // }
    
    protected void OnRoundStart(Battle battle)
    {
        if (!IsTapped && !IsNegated && Effect != null)
        {
            StartEffect();
        } 
    }


    public override void ToggleTap()
    {
        base.ToggleTap();
        if (IsTapped)
        {
            Battle.Player.Cost.Value += TapCostChange;
        }
        else
        {
            Battle.Player.Cost.Value += UnTapCostChange;
            StartEffect();
        }
    }
}