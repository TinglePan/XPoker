using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BalatrollCard: BaseUseCard
{
    public class BalatrollCardInputHandler : BaseInteractCardInputHandler<BalatrollCard>
    {
        public BalatrollCardInputHandler(BalatrollCard card) : base(card)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var node in Card.PlayerCardContainer.ContentNodes)
            {
                yield return node;
            }
        }

        protected override void Confirm()
        {
            if (SelectedCardNodes.Count != 0)
            {
                foreach (var selectedCardNode in SelectedCardNodes)
                {
                    selectedCardNode.IsSelected = false;
                    Card.Battle.Dealer.DealCardAndReplace(selectedCardNode);
                }
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
    
    public CardContainer PlayerCardContainer; 
    
    public BalatrollCard(Enums.CardSuit suit, Enums.CardRank rank) : base("Balatroll", 
        "Discard your hole cards at will, like in Balatro.", "res://Sprites/Cards/balatroll.png", 
        suit, rank, 1)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        PlayerCardContainer = GameMgr.SceneMgr.GetNode<CardContainer>("playerHoleCardContainer"); 
    }
    
    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void ChooseTargets()
    {
        var inputHandler =
            new BalatrollCardInputHandler(this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}