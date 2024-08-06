using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BalaTrollCard: BaseCard
{
    public class BalaTrollCardInputHandler : BaseCardSelectTargetInputHandlerWithConfirmConstraints
    {
        public BalaTrollCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return Helper.OriginateCard.GetProp<BalaTrollCardItemProp>().PlayerHoleCardContainer.CardNodes;
        }
    }
    
    public class BalaTrollCardItemProp : CardPropItem
    {
        public CardContainer PlayerHoleCardContainer; 
        public BalaTrollCardItemProp(BaseCard card, CardContainer playerHoleCardContainer) : base(card)
        {
            PlayerHoleCardContainer = playerHoleCardContainer;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }

        public override async Task Effect(List<CardNode> targets)
        {
            var tasks = new List<Task>();
            var tasks2 = new List<Task>();
            tasks.Add(base.Effect(targets));
            foreach (var selectedNode in targets)
            {
                var sourceContainer = selectedNode.CurrentContainer.Value;
                selectedNode.IsSelected = false;
                tasks.Add(selectedNode.AnimateLeaveField());
                tasks2.Add(Battle.Dealer.DrawCardIntoContainer((CardContainer)sourceContainer));
                await Utils.Wait(CardNode, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(tasks2);
        }

        protected override BaseInputHandler GetInputHandler()
        {
            return new BalaTrollCardInputHandler(GameMgr, CardNode);
        }
    }

    public class BalaTrollCardPiledCardItemReplaceProp : CardPropItemReplace
    {
        public BalaTrollCardPiledCardItemReplaceProp(BaseCard card) : base(card)
        {
            Cost.Value = 0;
            RankChangePerUse = 0;
        }
        
        public override async Task Effect(List<CardNode> targets)
        {
            var fromNode = CardNode;
            var toNode = targets[0];
            var fromContainer = (CardContainer)fromNode.CurrentContainer.Value;
            var toContainer = (CardContainer)toNode.CurrentContainer.Value;
            var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
            var toIndex = toContainer.ContentNodes.IndexOf(toNode);
            var tasks = new List<Task>
            {
                base.Effect(targets),
                toContainer.MoveCardNodeToContainer(toNode, fromContainer, fromIndex),
                fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex)
            };
            await Task.WhenAll(tasks);
        }

        public override List<CardNode> GetValidSelectTargets()
        {
            return Battle.Player.HoleCardContainer.CardNodes;
        }
    }

    public class BalaTrollCardPiledProp : CardPropPiled
    {
        public BalaTrollCardPiledProp(BaseCard card, int cardCount) : base(card, cardCount)
        {
        }
        
        public override async Task Open()
        {
            await base.Open();
            Battle.OpenedPiledCardContainer.CardContainers[1].OnAddContentNode += OnAddPiledCardNode;
            Battle.OpenedPiledCardContainer.CardContainers[1].OnRemoveContentNode += OnRemovePiledCardNode;
        }

        public override async Task Close()
        {
            await base.Close();
            Battle.OpenedPiledCardContainer.CardContainers[1].OnAddContentNode -= OnAddPiledCardNode;
            Battle.OpenedPiledCardContainer.CardContainers[1].OnRemoveContentNode -= OnRemovePiledCardNode;
        }

        protected void OnAddPiledCardNode(BaseContentNode node)
        {
            var card = (BaseCard)node.Content.Value;
            var usableProps = card.GetProps<BaseCardPropUsable>();
            Debug.Assert(usableProps.Count == 2, "More than 2 usable props, not expected");
            var piledReplaceProp = card.GetProp<BalaTrollCardPiledCardItemReplaceProp>(strict: true);
            if (piledReplaceProp == null)
            {
                piledReplaceProp = new BalaTrollCardPiledCardItemReplaceProp(card);
                card.Props.Add(typeof(BalaTrollCardPiledCardItemReplaceProp), piledReplaceProp);
            }
            foreach (var prop in usableProps)
            {
                prop.Enabled = false;
            }
            piledReplaceProp.Enabled = true;
        }
        
        protected void OnRemovePiledCardNode(BaseContentNode node)
        {
            var card = (BaseCard)node.Content.Value;
            var usableProps = card.GetProps<BaseCardPropUsable>();
            foreach (var prop in usableProps)
            {
                prop.Enabled = true;
            }
            var piledReplaceProp = card.GetProp<BalaTrollCardPiledCardItemReplaceProp>(strict: true);
            if (piledReplaceProp != null)
            {
                piledReplaceProp.Enabled = false;
            }
        }
    }
    
    public BalaTrollCard(CardDef def): base(def)
    {
    }
    
    protected override CardPropItem CreateItemProp()
    {
        return new BalaTrollCardItemProp(this, Battle.Player.HoleCardContainer);
    }

    protected override CardPropPiled CreatePiledProp()
    {
        return new BalaTrollCardPiledProp(this, Def.PileCardCountMax);
    }
}