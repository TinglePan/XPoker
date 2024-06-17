using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TheTieBreakerCard: BaseTapCard
{
    public TheTieBreakerCard(TapCardDef def): base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.HandEvaluator.IsSuitSecondComparer = true;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.HandEvaluator.IsSuitSecondComparer = false;
    }
}