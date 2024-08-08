using XCardGame.CardProperties;
using XCardGame.Common;

namespace XCardGame;

public class DarknessRuleCard: BaseCard
{
    
    public class DarknessCardRuleProp: CardPropRuleRoundReTrigger
    {
        public int Count;
        
        public DarknessCardRuleProp(BaseCard card): base(card)
        {
            Count = 0;
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
                Count = Utils.GetCardRankValue(Card.Rank.Value);
                Card.Battle.LastFlipCommunityCardCount += Count;
                IsEffectActive = true;
            }
        }

        public override void OnStopEffect()
        {
            if (IsEffectActive)
            {
                Card.Battle.LastFlipCommunityCardCount -= Count;
                IsEffectActive = false;
            }
        }
    }
    
    public DarknessRuleCard(CardDef def): base(def)
    {
    }

    protected override CardPropRule CreateRuleProp()
    {
        return new DarknessCardRuleProp(this);
    }
}