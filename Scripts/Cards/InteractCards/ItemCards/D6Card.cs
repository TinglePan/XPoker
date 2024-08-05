using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class D6Card: BaseCard
{

    public class D6CardItemProp : CardPropItem
    {
        public List<CardContainer> ValidCardContainers;
        
        public D6CardItemProp(BaseCard card, List<CardContainer> validCardContainers) : base(card)
        {
            ValidCardContainers = validCardContainers;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }
        
        public override async Task Effect(List<CardNode> targets)
        {
            var tasks = new List<Task>();
            tasks.Add(base.Effect(targets));
            foreach (var cardContainer in ValidCardContainers)
            {
                foreach (var cardNode in cardContainer.CardNodes)
                {
                    if (cardNode.FaceDirection.Value != Enums.CardFace.Up) continue;
                    tasks.Add(Battle.Dealer.DrawCardAndReplace(cardNode));
                    await Utils.Wait(CardNode, Configuration.AnimateCardTransformInterval);
                }
            }
            await Task.WhenAll(tasks);
        }
    }
    
    public D6Card(CardDef def): base(def)
    {
    }

    protected override CardPropItem CreateItemProp()
    {
        var validTargetContainers = new List<CardContainer>
        {
            Battle.Player.HoleCardContainer,
            Battle.CommunityCardContainer
        };
        return new D6CardItemProp(this, validTargetContainers);
    }
}