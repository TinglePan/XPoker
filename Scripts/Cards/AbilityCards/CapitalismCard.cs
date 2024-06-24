using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class CapitalismCard: BaseTapCard
{
    private int _count;
    
    public CapitalismCard(TapCardDef def): base(def)
    {
        
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        _count = Utils.GetCardRankValue(Rank.Value);
        battle.OnNewEnemy += OnNewEnemy;
        ChangeEntityDealCardCount(battle.Player, _count);
        ChangeEntityDealCardCount(battle.Enemy, _count);
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        ChangeEntityDealCardCount(battle.Player, -_count);
        ChangeEntityDealCardCount(battle.Enemy, -_count);
    }

    protected void ChangeEntityDealCardCount(BattleEntity entity, int count)
    {
        entity.DealCardCount += count;
        entity.HoleCardContainer.ExpectedContentNodeCount += count;
    }
    
    protected void OnNewEnemy(Battle battle, BattleEntity enemy)
    {
        ChangeEntityDealCardCount(enemy, _count);
    }
}