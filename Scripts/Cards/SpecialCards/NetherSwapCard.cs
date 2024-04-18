using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.SpecialCards;

public class NetherSwapCard: BaseSpecialCard
{
    
    public class NetherSwapCardInputHandler : BaseInputHandler
    {
        private PokerPlayer _player;
        private Container _playerHoleCardContainer;
        private Container _opponentHoleCardContainer;
        private Container _communityCardContainer;
        private CardNode _selectedCardNode;
        private SpecialCardNode _cardNode; 
        
        public NetherSwapCardInputHandler(GameMgr gameMgr, PokerPlayer player, Container playerHoleCardContainer, 
            Container opponentHoleCardContainer, Container communityHoleCardContainer, SpecialCardNode cardNode) : base(gameMgr)
        {
            _player = player;
            _playerHoleCardContainer = playerHoleCardContainer;
            _opponentHoleCardContainer = opponentHoleCardContainer;
            _communityCardContainer = communityHoleCardContainer;
            _cardNode = cardNode;
        }
        
        protected override void OnLeftMouseButtonPressed(Vector2 position)
        {
            GD.Print("Left Mouse Button Pressed handled by NetherSwapCardInputHandler");
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
            CheckClickOnHoleCardContainer(_playerHoleCardContainer, position);
            CheckClickOnHoleCardContainer(_opponentHoleCardContainer, position);
            CheckClickOnHoleCardContainer(_communityCardContainer, position);
        }
        
        public void CheckClickOnSelf(Vector2 position)
        {
            if (_cardNode.GetGlobalRect().HasPoint(position))
            {
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
        }

        protected void CheckClickOnHoleCardContainer(Container container, Vector2 position)
        {
            foreach (var child in container.GetChildren())
            {
                if (child is not PokerCardNode cardNode) continue;
                if (cardNode.GetGlobalRect().HasPoint(position))
                {
                    if (_selectedCardNode is { Card.Value: BasePokerCard pokerCard})
                    {
                        if (pokerCard != cardNode.Card.Value)
                        {
                            // swap cards
                            var tmpCard = new BasePokerCard(_selectedCardNode.Card.Value as BasePokerCard);
                            _selectedCardNode.Card.Value = cardNode.Card.Value;
                            cardNode.Card.Value = tmpCard;
                            // (_selectedCardNode.Card.Value, cardNode.Card.Value) = (cardNode.Card.Value, _selectedCardNode.Card.Value);
                            // keep the face direction of the swapped card
                            if (_selectedCardNode.Card.Value.Face.Value != cardNode.Card.Value.Face.Value)
                            {
                                _selectedCardNode.Card.Value.Flip();
                                cardNode.Card.Value.Flip();
                            }
                        }
                        _selectedCardNode.Card.Value.OnLoseFocus();
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
    }
    
    
    private Container _playerHoleCardContainer;
    private Container _opponentHoleCardContainer;
    private Container _communityCardContainer;
    
    public NetherSwapCard(GameMgr gameMgr, Container playerHoleCardContainer, Container opponentHoleCardContainer, 
        Container communityCardContainer, PokerPlayer owner, Enums.CardFace face) : base(gameMgr, "Nether swap",
        "Swap any two card in your hand, your opponent's hand or community cards.", owner, face,
        "res://Sprites/Cards/NetherSwap.png")
    {
        _playerHoleCardContainer = playerHoleCardContainer;
        _opponentHoleCardContainer = opponentHoleCardContainer;
        _communityCardContainer = communityCardContainer;
    }

    public override void Activate()
    {
        var inputHandler =
            new NetherSwapCardInputHandler(GameMgr, Owner, _playerHoleCardContainer, _opponentHoleCardContainer, _communityCardContainer, Node as SpecialCardNode);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}