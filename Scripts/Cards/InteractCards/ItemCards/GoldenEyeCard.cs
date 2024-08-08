using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class GoldenEyeCard: BaseCard
{
    public class GoldenEyeCardItemProp : CardPropItem
    {
        public List<CardContainer> ValidCardContainers;
        
        public GoldenEyeCardItemProp(BaseCard card, List<CardContainer> validCardContainers) : base(card)
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
            foreach (var cardContainer in ValidCardContainers)
            {
                foreach (var cardNode in cardContainer.CardNodes)
                {
                    if (cardNode.FaceDirection.Value == Enums.CardFace.Up) continue;
                    tasks.Add(GameMgr.AwaitAndDisableInput(cardNode.AnimateReveal(true, Configuration.RevealTweenTime)));
                    await Utils.Wait(CardNode, Configuration.AnimateCardTransformInterval);
                }
            }
            tasks.Add(Card.Battle.Dealer.DealCardPile.TopCardNode.AnimateReveal(true, Configuration.RevealTweenTime));
            await Task.WhenAll(tasks);
        }
    }

    public GoldenEyeCard(CardDef def): base(def)
    {
    }

    protected override CardPropItem CreateItemProp()
    {
        var validTargetContainers = new List<CardContainer>
        {
            Battle.Player.HoleCardContainer,
            Battle.Enemy.HoleCardContainer,
            Battle.CommunityCardContainer
        };
        return new GoldenEyeCardItemProp(this, validTargetContainers);
    }
}