using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.SpecialCards;

public class D6Card: BaseSpecialCard
{
    public class D6CardInputHandler : BaseInputHandler
    {
        private PokerPlayer _player;
        private Container _holeCardContainer;
        private CardNode _selectedCardNode;
        private SpecialCardNode _cardNode; 
        
        public D6CardInputHandler(GameMgr gameMgr, PokerPlayer player, Container holeCardContainer, SpecialCardNode cardNode) : base(gameMgr)
        {
            _player = player;
            _holeCardContainer = holeCardContainer;
            _cardNode = cardNode;
        }
        
        protected override void OnLeftMouseButtonPressed(Vector2 position)
        {
            GD.Print("Left Mouse Button Pressed handled by D6CardInputHandler");
            CheckClickOnHoleCardNodes(position);
            CheckClickOnSelf(position);
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

        public void CheckClickOnHoleCardNodes(Vector2 position)
        {
            foreach (var child in _holeCardContainer.GetChildren())
            {
                if (child is not CardNode cardNode) continue;
                if (cardNode.GetGlobalRect().HasPoint(position))
                {
                    if (_selectedCardNode is { Card.Value: BasePokerCard pokerCard})
                    {
                        var newCard = GameMgr.CurrentHand.DealingDeck.Deal(
                            facedDown: _selectedCardNode.Card.Value.Face.Value == Enums.CardFace.Down);
                        _player.SwapHoleCard(pokerCard, newCard);
                        _selectedCardNode = null;
                    }
                    else
                    {
                        cardNode.Card.Value.OnFocused();
                        _selectedCardNode = cardNode;
                    }
                }
            }
        }
        
        public void CheckClickOnSelf(Vector2 position)
        {
            if (_cardNode.GetGlobalRect().HasPoint(position))
            {
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
        }
    }
    private Container _holeCardContainer;
    
    public D6Card(GameMgr gameMgr, Container holeCardContainer, PokerPlayer player, Enums.CardFace face, string iconPath) : base(gameMgr, "Dice 6", "Reroll one of your hole card", player, face, iconPath)
    {
        _holeCardContainer = holeCardContainer;
    }

    public override void Activate()
    {
        D6CardInputHandler inputHandler =
            new D6CardInputHandler(GameMgr, Owner, _holeCardContainer, Node as SpecialCardNode);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}