using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BalaTrollCard: BaseCard
{
    public class BalaTrollCardItemProp : CardPropItemPiled
    {
        public CardContainer[] ValidCardContainers;
        public BalaTrollCardItemProp(BaseCard card, params CardContainer[] validCardContainers) : base(card)
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
        
        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var cardContainer in ValidCardContainers)
            {
                foreach (var node in cardContainer.ContentNodes)
                {
                    yield return (CardNode)node;
                }
            }
        }
    }

    public class BalaTrollCardPiledCardItemSwapProp : CardPropItem
    {
        public BalaTrollCardPiledCardItemSwapProp(BaseCard card) : base(card)
        {
            Cost.Value = 0;
            RankChangePerUse = 0;
        }
        
        public override bool CanUse()
        {
            if  (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }
    
        protected override BaseCardSelectTargetInputHandlerWithConfirmConstraints GetInputHandler()
        {
            return new BaseCardSelectTargetInputHandlerWithConfirmConstraints(GameMgr, CardNode, selectTargetCountLimit:1,
                getValidSelectTargetsFunc:GetValidSelectTargets);
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

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return Battle.Player.HoleCardContainer.CardNodes;
        }
    }

    public class BalaTrollCardPiledProp : CardPropPiled
    {
        public OpenedPiledCard OpenedPiledCard;
        public BalaTrollCardPiledProp(BaseCard card, int cardCount) : base(card, cardCount)
        {
        }
        
        public override async Task Open()
        {
            Battle.OpenedPiledCardContainer.CardContainers[1].OnAddContentNode += OnAddPiledCardNode;
            Battle.OpenedPiledCardContainer.CardContainers[1].OnRemoveContentNode += OnRemovePiledCardNode;
            await base.Open();
        }

        public override async Task Close()
        {
            await base.Close();
            Battle.OpenedPiledCardContainer.CardContainers[1].OnAddContentNode -= OnAddPiledCardNode;
            Battle.OpenedPiledCardContainer.CardContainers[1].OnRemoveContentNode -= OnRemovePiledCardNode;
        }

        public override async void OnEnterField()
        {
            var tasks = new List<Task>();
            for (int i = 0;i < CardCount; i++)
            {
                tasks.Add(Battle.Dealer.DrawCardToPile(PiledCardNode.CardPile));
            }
            await Task.WhenAll(tasks);
        }

        protected void OnAddPiledCardNode(BaseContentNode node)
        {
            var card = (BaseCard)node.Content.Value;
            var usableProps = card.GetProps<BaseCardPropUsable>();
            Debug.Assert(usableProps.Count <= 2, "More than 2 usable props, not expected");
            var piledReplaceProp = card.GetProp<BalaTrollCardPiledCardItemSwapProp>(strict: true);
            if (piledReplaceProp == null)
            {
                piledReplaceProp = new BalaTrollCardPiledCardItemSwapProp(card);
                card.Props.Add(typeof(BalaTrollCardPiledCardItemSwapProp), piledReplaceProp);
            }
            foreach (var prop in usableProps)
            {
                prop.Enabled = false;
            }
            piledReplaceProp.Enabled = true;
            ((CardNode)node).AdjustCostLabel();
            GD.Print($"enabled item swap prop to card {card}");
        }
        
        protected void OnRemovePiledCardNode(BaseContentNode node)
        {
            var card = (BaseCard)node.Content.Value;
            var usableProps = card.GetProps<BaseCardPropUsable>();
            foreach (var prop in usableProps)
            {
                prop.Enabled = true;
            }
            var piledReplaceProp = card.GetProp<BalaTrollCardPiledCardItemSwapProp>(strict: true);
            if (piledReplaceProp != null)
            {
                piledReplaceProp.Enabled = false;
                GD.Print($"disabled item swap prop to card {card}");
            }
            ((CardNode)node).AdjustCostLabel();
        }
    }
    
    public BalaTrollCard(CardDef def): base(def)
    {
    }
    
    protected override CardPropItem CreateItemProp()
    {
        return new BalaTrollCardItemProp(this, Battle.OpenedPiledCardContainer.CardContainers[1]);
    }
    
    protected override CardPropPiled CreatePiledProp()
    {
        return new BalaTrollCardPiledProp(this, Def.PileCardCountMax);
    }
}