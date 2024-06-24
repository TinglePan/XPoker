using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseTapCard: BaseAbilityCard, ITapCard
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
    }
    
    public override bool CanInteract()
    {
        var node = Node<CardNode>();
        if (node.Container.Value is CardContainer { AllowInteract: false })
        {
            return false;
        }
        return !IsTapped && Battle.Player.Cost.Value + TapCostChange <= Battle.Player.MaxCost.Value ||
                IsTapped && Battle.Player.Cost.Value + UnTapCostChange <= Battle.Player.MaxCost.Value;
    }

    public void StartEffect()
    {
        var node = Node<CardNode>();
        if (node.Container.Value is CardContainer { AllowInteract: true })
        {
            Battle.StartEffect(Effect);
        }
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Battle.OnRoundStart += OnRoundStart;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        Battle.OnRoundStart -= OnRoundStart;
        // NOTE: Effect manages its stop on its own. And check if its source card in in position to decide whether its effect is skipped.
        // battle.StopEffect(Effect);
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
    
    protected void OnRoundStart(Battle battle)
    {
        if (!IsTapped && !IsNegated && Effect != null)
        {
            StartEffect();
        } 
    }

}