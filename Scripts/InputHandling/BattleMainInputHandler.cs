using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.InputHandling;

public class BattleMainInputHandler: BaseInputHandler
{
    public Battle Battle;
    public BaseButton ProceedButton;
    public CardContainer FieldCardContainer;
    
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
        
        FieldCardContainer.ContentNodes.CollectionChanged += OnFieldCardNodesCollectionChanged;
        foreach (var cardNode in FieldCardContainer.ContentNodes)
        {
            cardNode.OnPressed += OnCardNodePressed;
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        ProceedButton.Pressed -= Battle.Proceed;
        FieldCardContainer.ContentNodes.CollectionChanged -= OnFieldCardNodesCollectionChanged;
        foreach (var cardNode in FieldCardContainer.ContentNodes)
        {
            cardNode.OnPressed -= OnCardNodePressed;
        }
    }
    
    protected void OnCardNodePressed(BaseContentNode<BaseCard> node)
    {
        var cardNode = (CardNode)node;
        if (node.Content.Value is IInteractCard interactCard && interactCard.CanInteract(cardNode))
        {
            interactCard.Interact(cardNode);
        }
    }
    
    protected void OnFieldCardNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnPressed += OnCardNodePressed;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnPressed -= OnCardNodePressed;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var node in FieldCardContainer.GetChildren())
                {
                    if (node is CardNode card)
                    {
                        card.OnPressed -= OnCardNodePressed;
                    }
                }
                break;
        }
    }
}