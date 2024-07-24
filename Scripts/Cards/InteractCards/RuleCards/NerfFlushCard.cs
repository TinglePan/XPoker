using System.Collections.Generic;
using XCardGame.Common;

namespace XCardGame;

public class NerfFlushCard: BaseRuleCard
{
    protected List<Enums.HandTier> NerfHandRanksInDescendingOrder;
    

    public NerfFlushCard(RuleCardDef def) : base(def)
    {
    }
    
    public override void OnStartEffect(Battle battle)
    {
        base.OnStartEffect(battle);
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

    public override void OnStopEffect(Battle battle)
    {
        base.OnStopEffect(battle);
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