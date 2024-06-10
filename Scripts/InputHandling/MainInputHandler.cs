using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.InputHandling;

public class MainInputHandler: BaseInputHandler
{
    public BaseButton ProceedButton;
    public CardContainer FieldCardContainer;
    
    public MainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        FieldCardContainer = GameMgr.SceneMgr.GetNodeById<CardContainer>("fieldCardContainer");
        ProceedButton = GameMgr.CurrentBattle.ProceedButton;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        ProceedButton.Pressed += GameMgr.CurrentBattle.Proceed;
        // ProceedButton.Pressed += AddSkillCard;
        FieldCardContainer.ContentNodes.CollectionChanged += OnFieldCardNodesCollectionChanged;
        foreach (var cardNode in FieldCardContainer.ContentNodes)
        {
            cardNode.OnPressed += ClickCard;
        }
    }

    // protected void AddSkillCard()
    // {
    //     var playerSkillCardContainer = GameMgr.CurrentBattle.Player.SkillCardContainer;
    //     playerSkillCardContainer.Contents.Add(new DualWieldSkillCard(Enums.CardSuit.Spades, Enums.CardRank.Two, GameMgr.CurrentBattle.Player));
    // }
    
    public override void OnExit()
    {
        base.OnExit();
        ProceedButton.Pressed -= GameMgr.CurrentBattle.Proceed;
        FieldCardContainer.ContentNodes.CollectionChanged -= OnFieldCardNodesCollectionChanged;
        foreach (var cardNode in FieldCardContainer.ContentNodes)
        {
            cardNode.OnPressed -= ClickCard;
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
                            cardNode.OnPressed += ClickCard;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnPressed -= ClickCard;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var node in FieldCardContainer.GetChildren())
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