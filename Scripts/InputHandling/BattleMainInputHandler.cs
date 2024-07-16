using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Ui;
using Battle = XCardGame.Scripts.Game.Battle;
using CardContainer = XCardGame.Scripts.Ui.CardContainer;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.InputHandling;

public class BattleMainInputHandler: BaseInputHandler
{
    public Battle Battle;
    public BaseButton ProceedButton;
    public List<CardContainer> InteractCardContainers;
    
    public BattleMainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Battle = GameMgr.CurrentBattle;
        ProceedButton = GameMgr.CurrentBattle.BigButton;
        
        ProceedButton.Pressed += Battle.Proceed;
        if (ProceedButton is Button button)
        {
            button.Text = "Proceed...";
        }

        InteractCardContainers = new List<CardContainer>()
        {
            Battle.ItemCardContainer,
            Battle.RuleCardContainer
        };

        foreach (var cardContainer in InteractCardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged += OnInteractCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed += OnCardNodePressed;
            }
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        ProceedButton.Pressed -= Battle.Proceed;
        foreach (var cardContainer in InteractCardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged -= OnInteractCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed -= OnCardNodePressed;
            }
        }
    }
    
    protected void OnCardNodePressed(BaseContentNode node, MouseButton mouseButton)
    {
        if (mouseButton == MouseButton.Left)
        {
            var cardNode = (CardNode)node;
            if (node.Content.Value is IInteractCard interactCard && interactCard.CanInteract(cardNode))
            {
                interactCard.Interact(cardNode);
            }
        }
    }
    
    protected void OnInteractCardNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnMousePressed += OnCardNodePressed;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnMousePressed -= OnCardNodePressed;
                        }
                    }
                break;
        }
    }
}