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
        battle.Player.DealCardCount += _count;
        battle.Enemy.DealCardCount += _count;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.Player.DealCardCount -= _count;
        battle.Enemy.DealCardCount -= _count;
    }
}