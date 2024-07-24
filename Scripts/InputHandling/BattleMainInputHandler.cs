using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;
using XCardGame.Ui;
using CardContainer = XCardGame.Ui.CardContainer;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public class BattleMainInputHandler: BaseInputHandler
{
    public Battle Battle;
    public BaseButton ProceedButton;
    public List<CardContainer> InteractCardContainers;
    
    public BattleMainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        // GD.Print("await and disable input 1");
        ReceiveInput = false;
        ProceedButton.Disabled = true;
        await task;
        ProceedButton.Disabled = false;
        ReceiveInput = true;
        // GD.Print("await and disable input 2");
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
        GD.Print($"on card node {node} pressed: {ReceiveInput}.");
        if (ReceiveInput)
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