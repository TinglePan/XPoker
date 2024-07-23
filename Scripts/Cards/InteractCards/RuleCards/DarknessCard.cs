﻿using XCardGame.Scripts.Common;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class DarknessCard: BaseRuleCard
{
    protected int Count;
    
    public DarknessCard(RuleCardDef def): base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Count = Utils.GetCardRankValue(Rank.Value);
            battle.FaceDownCommunityCardCount += Count;
            AlreadyFunctioning = true;
        }
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            battle.FaceDownCommunityCardCount -= Count;
            AlreadyFunctioning = false;
        }
    }
}