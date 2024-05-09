using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BalatrollCard: BaseActivatableAbilityCard
{

    public class BalatrollCardInputHandler : BaseInputHandler
    {

        private BalatrollCard _card;
        
        private List<CardNode> _selectedCardNodes;
        
        public BalatrollCardInputHandler(GameMgr gameMgr, BalatrollCard card) : base(gameMgr)
        {
            _card = card;
            _selectedCardNodes = new List<CardNode>();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            foreach (var card in _card.PlayerCardContainer.Cards)
            {
                card.Node.OnPressed += ClickCard;
            }
            _card.Node.OnPressed += ClickSelf;
            GD.Print("Enter BalatrollCardInputHandler");
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var card in _card.PlayerCardContainer.Cards)
            {
                card.Node.OnPressed -= ClickCard;
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
            if (_selectedCardNodes.Count != 0)
            {
                foreach (var selectedCardNode in _selectedCardNodes)
                {
                    selectedCardNode.Card.Value.IsSelected.Value = false;
                    var newCard = GameMgr.CurrentBattle.DealingDeck.Deal();
                    newCard.Face.Value = Enums.CardFace.Up;
                    selectedCardNode.Card.Value = newCard;
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
                node.Card.Value.IsSelected.Value = false;
                _selectedCardNodes.Remove(node);
            }
            else
            {
                _selectedCardNodes.Add(node);
                node.Card.Value.IsSelected.Value = true;
            }
        }
    }
    
    public CardContainer PlayerCardContainer; 
    
    public BalatrollCard(GameMgr gameMgr, Enums.CardFace face, Enums.CardSuit suit, BattleEntity owner,
        int cost, int coolDown, bool isQuick = false) : base(gameMgr, "Balatroll", 
        "Troll version of Balatro.", face, suit, "res://Sprites/Cards/balatroll.png", cost, coolDown,
        isQuick, owner)
    {
        PlayerCardContainer = gameMgr.UiMgr.GetNodeById<CardContainer>("playerHoleCardContainer");
    }
    
    public override void Activate()
    {
        var inputHandler =
            new BalatrollCardInputHandler(GameMgr, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}