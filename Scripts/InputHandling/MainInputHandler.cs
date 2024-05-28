using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.InputHandling;

public class MainInputHandler: BaseInputHandler
{
    public BaseButton ProceedButton;
    public CardContainer PlayerAbilityCardContainer;
    public CardContainer FieldCardContainer;
    
    public MainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        PlayerAbilityCardContainer = GameMgr.SceneMgr.GetNodeById<CardContainer>("playerAbilityCardContainer");
        ProceedButton = SceneMgr.GetNode<BaseButton>("proceedButton");
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        ProceedButton.Pressed += GameMgr.CurrentBattle.Proceed;
        PlayerAbilityCardContainer.Contents.CollectionChanged += OnAbilityCardCollectionChanged;
        foreach (var card in PlayerAbilityCardContainer.Contents)
        {
            card.Node.OnPressed += ClickCard;
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        ProceedButton.Pressed -= GameMgr.CurrentBattle.Proceed;
        PlayerAbilityCardContainer.Contents.CollectionChanged -= OnAbilityCardCollectionChanged;
        foreach (var card in PlayerAbilityCardContainer.Contents)
        {
            card.Node.OnPressed -= ClickCard;
        }
    }
    
    protected void ClickCard(CardNode node)
    {
        GD.Print($"ClickCard {node.Content.Value}");
        if (node.Content.Value is BaseInteractCard card && card.CanInteract())
        {
            card.Interact();
        }
    }
    
    protected void OnAbilityCardCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is BaseCard card)
                        {
                            card.Node.OnPressed += ClickCard;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is BaseCard card)
                        {
                            card.Node.OnPressed -= ClickCard;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var node in PlayerAbilityCardContainer.GetChildren())
                {
                    if (node is CardNode card)
                    {
                        card.OnPressed -= ClickCard;
                    }
                }
                break;
        }
    }
}