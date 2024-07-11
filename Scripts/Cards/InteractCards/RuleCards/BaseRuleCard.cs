using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards;

public class BaseRuleCard: BaseInteractCard
{
    
    public BaseRuleCard(RuleCardDef def) : base(def)
    {
    }
    
    public override bool CanInteract(CardNode node)
    {
        if (!base.CanInteract(node)) return false;
        var ruleCardDef = (RuleCardDef)Def;
        if (node.Container.Value is CardContainer cardContainer && cardContainer.ExpectedInteractCardDefType != typeof(RuleCardDef)) return false;
        if (!node.IsTapped.Value)
        {
            // Seal
            if (Battle.Player.Energy.Value < ruleCardDef.SealCost) return false;
        }
        return true;
    }

    public override void Interact(CardNode node)
    {
        var ruleCardDef = (RuleCardDef)Def;
        if (!node.IsTapped.Value)
        {
            // Seal
            Battle.Player.Energy.Value -=  ruleCardDef.SealCost;
        }
        node.TweenTap(!node.IsTapped.Value, Configuration.TapTweenTime);
    }
    
    public override void OnStart(Battle battle)
    {
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            battle.OnRoundEnd += OnRoundEnd;
            AlreadyFunctioning = true;
        }
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            battle.OnRoundEnd -= OnRoundEnd;
            AlreadyFunctioning = false;
        }
    }

    protected void OnRoundEnd(Battle battle)
    {
        var ruleCardDef = (RuleCardDef)Def;
        if (IsFunctioning())
        {
            ChangeRank(ruleCardDef.RankChangePerTurn);
        }
    }
}