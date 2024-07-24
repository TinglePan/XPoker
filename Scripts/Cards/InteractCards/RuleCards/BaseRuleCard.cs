using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BaseRuleCard: BaseInteractCard
{
    
    public BaseRuleCard(RuleCardDef def) : base(def)
    {
    }
    
    public override bool CanInteract(CardNode node)
    {
        if (!base.CanInteract(node)) return false;
        var ruleCardDef = (RuleCardDef)Def;
        if (node.CurrentContainer.Value is CardContainer cardContainer && cardContainer.ExpectedInteractCardDefType != typeof(RuleCardDef)) return false;
        if (!node.IsTapped.Value)
        {
            // Seal
            if (Battle.Player.Energy.Value < ruleCardDef.Cost) return false;
        }
        return true;
    }

    public override void Interact(CardNode node)
    {
        var ruleCardDef = (RuleCardDef)Def;
        if (!node.IsTapped.Value)
        {
            // Seal
            Battle.Player.Energy.Value -=  ruleCardDef.Cost;
        }
        node.AnimateTap(!node.IsTapped.Value, Configuration.TapTweenTime);
    }
    
    public override void OnStartEffect(Battle battle)
    {
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            battle.OnRoundStart += OnRoundStart;
            battle.OnRoundEnd += OnRoundEnd;
            AlreadyFunctioning = true;
        }
    }
    
    public override void OnStopEffect(Battle battle)
    {
        base.OnStopEffect(battle);
        if (AlreadyFunctioning)
        {
            battle.OnRoundStart -= OnRoundStart;
            battle.OnRoundEnd -= OnRoundEnd;
            AlreadyFunctioning = false;
        }
    }

    protected void OnRoundStart(Battle battle)
    {
        var ruleCardDef = (RuleCardDef)Def;
        var cardNode = Node<CardNode>();
        if (ruleCardDef.AutoUnSeal && cardNode.IsTapped.Value)
        {
            cardNode.AnimateTap(false, Configuration.TapTweenTime);
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