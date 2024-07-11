using XCardGame.Scripts.Common;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class CapitalismCard: BaseRuleCard
{
    protected int Count;
    
    public CapitalismCard(InteractCardDef def): base(def)
    {
        
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Count = Utils.GetCardRankValue(Rank.Value);
            battle.OnNewEnemy += OnNewEnemy;
            ChangeEntityDealCardCount(battle.Player, Count);
            ChangeEntityDealCardCount(battle.Enemy, Count);
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            ChangeEntityDealCardCount(battle.Player, -Count);
            ChangeEntityDealCardCount(battle.Enemy, -Count);
            AlreadyFunctioning = false;
        }
    }

    protected void ChangeEntityDealCardCount(BattleEntity entity, int count)
    {
        entity.DealCardCount += count;
        entity.HoleCardContainer.ExpectedContentNodeCount += count;
    }
    
    protected void OnNewEnemy(Battle battle, BattleEntity enemy)
    {
        ChangeEntityDealCardCount(enemy, Count);
    }
}