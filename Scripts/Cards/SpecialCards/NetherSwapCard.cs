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

        private NetherSwapCard _card;
        
        private CardNode _selectedCardNode;
        
        public NetherSwapCardInputHandler(GameMgr gameMgr, NetherSwapCard card) : base(gameMgr)
        {
            _card = card;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            foreach (var holeCard in _card.PlayerHoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed += ClickHoleCard;
            }
            foreach (var holeCard in _card.OpponentHoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed += ClickHoleCard;
            }
            
            foreach (var holeCard in _card.CommunityCardContainer.Cards)
            {
                holeCard.Node.OnPressed += ClickHoleCard;
            }
            _card.Node.OnPressed += ClickSelf;
            GD.Print("Enter NetherSwapCardInputHandler");
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var holeCard in _card.PlayerHoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed -= ClickHoleCard;
            }
            foreach (var holeCard in _card.OpponentHoleCardContainer.Cards)
            {
                holeCard.Node.OnPressed -= ClickHoleCard;
            }
            foreach (var holeCard in _card.CommunityCardContainer.Cards)
            {
                holeCard.Node.OnPressed -= ClickHoleCard;
            }
            _card.Node.OnPressed -= ClickSelf;
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
        
        protected void ClickSelf(CardNode node)
        {
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
        
        protected void ClickHoleCard(CardNode node)
        {
            if (_selectedCardNode is { Card.Value: BasePokerCard fromCard})
            {
                if (node != _selectedCardNode)
                {
                    var toCard = node.Card.Value;
                    var fromContainer = _selectedCardNode.Container;
                    var toContainer = node.Container;
                    var fromIndex = fromContainer.Cards.IndexOf(fromCard);
                    var toIndex = toContainer.Cards.IndexOf(toCard);
                    toContainer.Cards[toIndex] = fromCard;
                    fromContainer.Cards[fromIndex] = toCard;
                    
                    // keep face value in sync with their target container
                    if (fromCard.Face.Value != toCard.Face.Value)
                    {
                        (fromCard.Face.Value, toCard.Face.Value) = (toCard.Face.Value, fromCard.Face.Value);
                    }
                }
                _selectedCardNode.Card.Value.OnLoseFocus();
                _selectedCardNode = null;
            }
            else
            {
                node.Card.Value.OnFocused();
                _selectedCardNode = node;
            }
        }
    }
    
    public CardContainer PlayerHoleCardContainer;
    public CardContainer OpponentHoleCardContainer;
    public CardContainer CommunityCardContainer;
    
    public NetherSwapCard(GameMgr gameMgr, CardContainer playerHoleCardContainer, CardContainer opponentHoleCardContainer, 
        CardContainer communityCardContainer, PokerPlayer owner, Enums.CardFace face) : base(gameMgr, "Nether swap",
        "Swap any two card in your hand, your opponent's hand or community cards.", owner, face,
        "res://Sprites/Cards/NetherSwap.png")
    {
        PlayerHoleCardContainer = playerHoleCardContainer;
        OpponentHoleCardContainer = opponentHoleCardContainer;
        CommunityCardContainer = communityCardContainer;
    }

    public override void Activate()
    {
        var inputHandler =
            new NetherSwapCardInputHandler(GameMgr, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}