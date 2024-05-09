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
        _abilityCardContainer.Cards.CollectionChanged += OnAbilityCardCollectionChanged;
        foreach (var card in _abilityCardContainer.Cards)
        {
            card.Node.OnPressed += ClickCard;
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        _abilityCardContainer.Cards.CollectionChanged -= OnAbilityCardCollectionChanged;
        foreach (var card in _abilityCardContainer.Cards)
        {
            card.Node.OnPressed -= ClickCard;
        }
    }
    
    protected void ClickCard(CardNode node)
    {
        GD.Print($"ClickCard {node.Card.Value}");
        if (node.Card.Value is IActivatableCard card && card.CanActivate())
        {
            card.Activate();
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
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems != null && args.OldItems[0] is BaseCard replacedCard && args.NewItems != null)
                {
                    var replacedCardNode = _abilityCardContainer.GetChild<CardNode>(args.OldStartingIndex);
                    replacedCardNode.Card.Value = args.NewItems[0] as BaseCard;
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var node in _abilityCardContainer.GetChildren())
                {
                    if (node is CardNode card)
                    {
                        card.OnPressed -= ClickCard;
                    }
                }
                _abilityCardContainer.ClearChildren();
                break;
        }
    }
}