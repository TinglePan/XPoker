using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class NerfFlushCard: BaseRuleCard
{
    protected List<Enums.HandTier> NerfHandRanksInDescendingOrder;
    

    public NerfFlushCard(InteractCardDef def) : base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            var flushIndex = battle.HandTierOrderDescend.IndexOf(Enums.HandTier.Flush);
            if (flushIndex < battle.HandTierOrderDescend.Count - 1)
            {
                (battle.HandTierOrderDescend[flushIndex], battle.HandTierOrderDescend[flushIndex + 1]) = (battle.HandTierOrderDescend[flushIndex + 1], battle.HandTierOrderDescend[flushIndex]);
            }

            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            var flushIndex = battle.HandTierOrderDescend.IndexOf(Enums.HandTier.Flush);
            if (flushIndex > 0)
            {
                (battle.HandTierOrderDescend[flushIndex], battle.HandTierOrderDescend[flushIndex - 1]) = (battle.HandTierOrderDescend[flushIndex - 1], battle.HandTierOrderDescend[flushIndex]);
            }

            AlreadyFunctioning = false;
        }
    }
}