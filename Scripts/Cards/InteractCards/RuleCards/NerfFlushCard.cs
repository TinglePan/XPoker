using System.Collections.Generic;
using XCardGame.CardProperties;
using XCardGame.Common;

namespace XCardGame;

public class NerfFlushCard: BaseCard
{
    public class NerfFlushCardRuleProp: CardPropRule
    {
        public List<Enums.HandTier> HandRanksInDescendingOrderBeforeNerf;
        
        public NerfFlushCardRuleProp(BaseCard card): base(card)
        {
            HandRanksInDescendingOrderBeforeNerf = null;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }
        
        public override void OnStartEffect()
        {
            if (!IsEffectActive)
            {
                
                HandRanksInDescendingOrderBeforeNerf = new List<Enums.HandTier>(Battle.HandTierOrderDescend);
                
                var flushIndex = Battle.HandTierOrderDescend.IndexOf(Enums.HandTier.Flush);
                if (flushIndex < Battle.HandTierOrderDescend.Count - 1)
                {
                    (Battle.HandTierOrderDescend[flushIndex], Battle.HandTierOrderDescend[flushIndex + 1]) = (Battle.HandTierOrderDescend[flushIndex + 1], Battle.HandTierOrderDescend[flushIndex]);
                }
                IsEffectActive = true;
            }
        }

        public override void OnStopEffect()
        {
            if (IsEffectActive)
            {
                for (int i = 0; i < HandRanksInDescendingOrderBeforeNerf.Count; i++)
                {
                    if (HandRanksInDescendingOrderBeforeNerf[i] != Battle.HandTierOrderDescend[i])
                    {
                        Battle.HandTierOrderDescend[i] = HandRanksInDescendingOrderBeforeNerf[i];
                    }
                }
            }
        }
    }

    public NerfFlushCard(CardDef def) : base(def)
    {
    }
    
    protected override CardPropRule CreateRuleProp()
    {
        return new NerfFlushCardRuleProp(this);
    }
}