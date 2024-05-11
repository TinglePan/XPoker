using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class NetherSwapCard: BaseActivatableCard
{
    public class NetherSwapCardInputHandler : BaseInputHandler
    {

        private NetherSwapCard _card;
        
        private List<CardNode> _selectedCardNodes;
        
        public NetherSwapCardInputHandler(GameMgr gameMgr, NetherSwapCard card) : base(gameMgr)
        {
            _card = card;
            _selectedCardNodes = new List<CardNode>();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            foreach (var cardContainer in _card.CardContainers)
            {
                foreach (var card in cardContainer.Cards)
                {
                    card.Node.OnPressed += ClickCard;
                }
            }
            _card.Node.OnPressed += ClickSelf;
            // GD.Print("Enter NetherSwapCardInputHandler");
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var cardContainer in _card.CardContainers)
            {
                foreach (var card in cardContainer.Cards)
                {
                    card.Node.OnPressed -= ClickCard;
                }
            }
            _card.Node.OnPressed -= ClickSelf;
        }
        
        protected override void OnRightMouseButtonPressed(Vector2 position)
        {
            _card.AfterCanceled();
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
        
        protected override void OnActionPressed(InputEventAction action)
        {
            if (action.Action == "ui_escape")
            {
                _card.AfterCanceled();
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
        }
        
        protected void ClickSelf(CardNode node)
        {
            if (_selectedCardNodes.Count == 2)
            {
                var fromNode = _selectedCardNodes[0];
                var toNode = _selectedCardNodes[1];
                var fromCard = fromNode.Card.Value;
                var toCard = toNode.Card.Value;
                var fromContainer = fromNode.Container;
                var toContainer = toNode.Container;
                var fromIndex = fromContainer.Cards.IndexOf(fromCard);
                var toIndex = toContainer.Cards.IndexOf(toCard);
                toContainer.Cards[toIndex] = fromCard;
                fromCard.Node.IsSelected.Value = false;
                fromContainer.Cards[fromIndex] = toCard;
                toCard.Node.IsSelected.Value = false;
                // keep face value in sync with their target container
                if (fromCard.Face.Value != toCard.Face.Value)
                {
                    (fromCard.Face.Value, toCard.Face.Value) = (toCard.Face.Value, fromCard.Face.Value);
                }
                _selectedCardNodes.Clear();
                _card.AfterEffect();
            }
            else
            {
                _card.AfterCanceled();
            }
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
        
        protected void ClickCard(CardNode node)
        {
            if (_selectedCardNodes.Contains(node))
            {
                node.IsSelected.Value = false;
                _selectedCardNodes.Remove(node);
            }
            else
            {
                _selectedCardNodes.Add(node);
                node.IsSelected.Value = true;
            }
        }
    }
    
    public List<CardContainer> CardContainers;
    
    public NetherSwapCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int coolDown = 2, bool isQuick = false, BattleEntity owner = null) : base("Nether swap",
        "Swap any two cards you can see.", "res://Sprites/Cards/nether_swap.png", face, suit, rank,
        cost, coolDown, isQuick, owner)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }

    public override void Activate()
    {
        var inputHandler =
            new NetherSwapCardInputHandler(GameMgr, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}