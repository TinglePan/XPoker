using XCardGame.Scripts.Common;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class SocialismCard: BaseTapCard
{
    private int _count;
    
    public SocialismCard(InteractCardDef def): base(def)
    {
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            _count = Utils.GetCardRankValue(Rank.Value);
            battle.DealCommunityCardCount += _count;
            battle.CommunityCardContainer.ExpectedContentNodeCount += _count;
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            battle.DealCommunityCardCount -= _count;
            battle.CommunityCardContainer.ExpectedContentNodeCount -= _count;
            AlreadyFunctioning = false;
        }
    }
}