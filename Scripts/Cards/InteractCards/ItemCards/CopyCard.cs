using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class CopyCard : BaseCard
{
    public class CopyCardItemReplaceProp : CardPropItemReplace
    {
        public List<CardContainer> ValidCardContainers;
        public CopyCardItemReplaceProp(BaseCard card, List<CardContainer> validTargetContainers) : base(card)
        {
            ValidCardContainers = validTargetContainers;
        }

        protected override BaseInputHandler GetInputHandler()
        {
            return new BaseCardSelectTargetInputHandlerWithConfirmConstraints(GameMgr, CardNode,
                selectTargetCountLimit:1, getValidSelectTargetsFunc:GetValidSelectTargets);
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return ValidCardContainers.SelectMany(x => x.CardNodes).Where(x => x.FaceDirection.Value == Enums.CardFace.Up);
        }
    }
    
    protected static CardDef CreateDefFromCopiedCard(CardDef def, BaseCard card)
    {
        var res = def.Clone<CardDef>();
        res.Rank = card.Def.Rank;
        res.Suit = card.Def.Suit;
        return res;
    }
        
    public CopyCard(CardDef def, BaseCard target) : base(CreateDefFromCopiedCard(def, target))
    {
    }

    protected override CardPropItem CreateItemProp()
    {
        var validTargetContainers = new List<CardContainer>
        {
            Battle.Player.HoleCardContainer,
            Battle.CommunityCardContainer
        };
        return new CopyCardItemReplaceProp(this, validTargetContainers);
    }
}