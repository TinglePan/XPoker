using System;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class D6Card: BaseAbilityCard
{
    public class D6CardInputHandler : BaseInputHandler
    {
        private CardNode _selectedCardNode;
        
        private D6Card _card;
        
        public D6CardInputHandler(GameMgr gameMgr, D6Card card) : base(gameMgr)
        {
            _card = card;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            foreach (var holeCard in _card.HoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed += ClickHoleCard;
            }

            _card.Node.OnPressed += ClickSelf;
            GD.Print("Enter D6CardInputHandler");
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var holeCard in _card.HoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed -= ClickHoleCard;
            }
            _card.Node.OnPressed -= ClickSelf;
        }

        protected void ClickHoleCard(CardNode node)
        {
            if (_selectedCardNode is { Card.Value: BasePokerCard pokerCard})
            {
                var newCard = GameMgr.CurrentBattle.DealingDeck.Deal();
                newCard.Face.Value = _selectedCardNode.Card.Value.Face.Value;
                _card.HoleCardContainer.Cards[_card.HoleCardContainer.Cards.IndexOf(pokerCard)] = newCard;
                _selectedCardNode = null;
            }
            else
            {
                node.Card.Value.OnFocused();
                _selectedCardNode = node;
            }
        }

        protected void ClickSelf(CardNode node)
        {
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
        
        
        protected override void OnRightMouseButtonPressed(Vector2 position)
        {
            if (_selectedCardNode is { Card.Value: BasePokerCard pokerCard})
            {
                pokerCard.OnLoseFocus();
                _selectedCardNode = null;
            }
            else
            {
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
        }
        
        protected override void OnActionPressed(InputEventAction action)
        {
            if (action.Action == "ui_escape")
            {
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
        }
    }
    
    public CardContainer HoleCardContainer;
    
    public D6Card(GameMgr gameMgr, Enums.CardFace face, GameLogic.BattleEntity owner) : base(gameMgr, "Dice 6", "Reroll one of your hole card", face, owner,
        "res://Sprites/Cards/D6.png")
    {
        HoleCardContainer = GameMgr.UiMgr.GetNodeById<CardContainer>("playerHoleCardContainer");
    }

    public override void Activate()
    {
        D6CardInputHandler inputHandler =
            new D6CardInputHandler(GameMgr, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}