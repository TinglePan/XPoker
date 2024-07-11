using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class TheTieBreakerCard: BaseRuleCard
{
    public TheTieBreakerCard(RuleCardDef def): base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            battle.HandEvaluator.IsSuitSecondComparer = true;
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            battle.HandEvaluator.IsSuitSecondComparer = false;
            AlreadyFunctioning = false;
        }
    }
}