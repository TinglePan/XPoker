using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class CardContainer: Container, IManagedUi
{
    [Export] public string Identifier { get; set; }
    
    [Export]
    public PackedScene CardPrefab;

    public ObservableCollection<BaseCard> Cards;
    public GameMgr GameMgr { get; private set; }
    public UiMgr UiMgr { get; private set; }
    
    
    public override void _Ready()
    {
        ClearChildren();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        UiMgr = GetNode<UiMgr>("/root/UiMgr");
        UiMgr.Register(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Cards != null)
        {
            Cards.CollectionChanged -= OnCardsChanged;
        }
    }

    public void Setup(Dictionary<string, object> args)
    {
        if (args["cards"] is ObservableCollection<BaseCard> cards && cards != Cards)
        {
            Cards = cards;
            cards.CollectionChanged += OnCardsChanged;
        }
    }
    
    protected void OnCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        var cardNode = CardPrefab.Instantiate<CardNode>();
                        AddChild(cardNode);
                        cardNode.Setup(new Dictionary<string, object>()
                        {
                            { "card", t },
                            { "container", this }
                        });
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null && args.OldItems[0] is BaseCard removedCard)
                {
                    var removedCardNode = GetChild<CardNode>(args.OldStartingIndex);
                    removedCardNode.QueueFree();
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems != null && args.OldItems[0] is BaseCard replacedCard && args.NewItems != null)
                {
                    var replacedCardNode = GetChild<CardNode>(args.OldStartingIndex);
                    replacedCardNode.Card.Value = args.NewItems[0] as BaseCard;
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                ClearChildren();
                break;
        }
    }
    
    protected void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }
}