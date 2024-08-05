using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Ui;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public class MagicalHatCard: BaseCard
{
    public class MagicalHatCardInputHandler : BaseCardSelectTargetInputHandlerWithConfirmConstraints
    {
        public MagicalHatCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node, 2, mustFullTargets: true)
        {
        }
        
        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return Helper.OriginateCard.GetProp<MagicalHatCardItemProp>().ValidCardContainers.SelectMany(x => x.CardNodes);
        }
    }
    
    public class MagicalHatCardItemProp : CardPropItem
    {
        public List<CardContainer> ValidCardContainers;

        public MagicalHatCardItemProp(BaseCard card, List<CardContainer> validCardContainers) : base(card)
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
            var fromNode = targets[0];
            var toNode = targets[1];
            var fromContainer = (CardContainer)fromNode.CurrentContainer.Value;
            var toContainer = (CardContainer)toNode.CurrentContainer.Value;
            var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
            var toIndex = toContainer.ContentNodes.IndexOf(toNode);
            tasks.Add(base.Effect(targets));
            tasks.Add(fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex));
            tasks.Add(toContainer.MoveCardNodeToContainer(toNode, fromContainer, fromIndex));
            await Task.WhenAll(tasks);
        }

        protected override BaseInputHandler GetInputHandler()
        {
            return new MagicalHatCardInputHandler(GameMgr, CardNode);
        }
    }
    
    public MagicalHatCard(CardDef def) : base(def)
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
        return new MagicalHatCardItemProp(this, validTargetContainers);
    }
}