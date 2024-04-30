using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.InputHandling;

public class MainInputHandler: BaseInputHandler
{
    
    private CardContainer _abilityCardContainer;
    
    public MainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        _abilityCardContainer = GameMgr.UiMgr.GetNodeById<CardContainer>("abilityCardContainer");
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        _abilityCardContainer.Cards.CollectionChanged += OnSpecialCardCollectionChanged;
        foreach (var card in _abilityCardContainer.Cards)
        {
            card.Node.OnPressed += ClickCard;
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        _abilityCardContainer.Cards.CollectionChanged -= OnSpecialCardCollectionChanged;
        foreach (var card in _abilityCardContainer.Cards)
        {
            card.Node.OnPressed -= ClickCard;
        }
    }
    
    protected void ClickCard(CardNode node)
    {
        GD.Print($"ClickCard {node.Card.Value}");
        if (node.Card.Value is BaseAbilityCard specialCard)
        {
            specialCard.Activate();
        }
    }
    
    protected void OnSpecialCardCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
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
        }
    }
}