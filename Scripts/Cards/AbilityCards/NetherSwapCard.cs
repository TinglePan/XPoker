using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class NetherSwapCard: BaseUseCard
{
    public class NetherSwapCardInputHandler : BaseInteractCardInputHandler<NetherSwapCard>
    {
        public NetherSwapCardInputHandler(NetherSwapCard card) : base(card)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var cardContainer in Card.CardContainers)
            {
                foreach (var node in cardContainer.ContentNodes)
                {
                    yield return node;
                }
            }
        }

        protected override void Confirm()
        {
            if (SelectedCardNodes.Count == 2)
            {
                var fromNode = SelectedCardNodes[0];
                var toNode = SelectedCardNodes[1];
                var fromContainer = fromNode.Container;
                var toContainer = toNode.Container;
                var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
                var toIndex = toContainer.ContentNodes.IndexOf(toNode);
                toContainer.ReplaceContentNode(toIndex, fromNode, Configuration.SwapCardTweenTime);
                fromContainer.ReplaceContentNode(fromIndex, toNode, Configuration.SwapCardTweenTime);
                fromNode.IsSelected = false;
                toNode.IsSelected = false;
                SelectedCardNodes.Clear();
                Card.Use();
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public List<CardContainer> CardContainers;
    
    public NetherSwapCard(Enums.CardSuit suit, Enums.CardRank rank) : base("Nether swap",
        "Swap any two cards you can see.", "res://Sprites/Cards/nether_swap.png", suit, rank, 1)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.SceneMgr.GetNodes<CardContainer>("markerCardContainer");
    }

    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }
    
    public override void ChooseTargets()
    {
        var inputHandler =
            new NetherSwapCardInputHandler(this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}