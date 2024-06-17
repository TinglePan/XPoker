using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class SocialismCard: BaseTapCard
{
    private int _count;
    
    public SocialismCard(TapCardDef def): base(def)
    {
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        _count = Utils.GetCardRankValue(Rank.Value);
        battle.DealCommunityCardCount += _count;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.DealCommunityCardCount -= _count;
    }
}