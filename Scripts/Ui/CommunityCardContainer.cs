using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;

namespace XCardGame.Scripts.Ui;

public partial class CommunityCardContainer: HBoxContainer, ISetup
{
    [Export]
    public PackedScene CardPrefab;
    
    public Hand Hand;
    
    
    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is PokerCardNode cardNode)
            {
                cardNode.QueueFree();
            }
        }
    }
    
    
    public void Setup(Dictionary<string, object> args)
    {
        Hand = args["hand"] as Hand;
        if (Hand != null)
        {
            Hand.CommunityCards.CollectionChanged += OnCommunityCardsChanged;
        }
    }
    
    protected void OnCommunityCardsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        var cardNode = CardPrefab.Instantiate<PokerCardNode>();
                        cardNode.Setup(new Dictionary<string, object>()
                        {
                            { "card", t },
                        });
                        AddChild(cardNode);
                    }

                break;
            case NotifyCollectionChangedAction.Remove:
				
                foreach (var child in GetChildren())
                {
                    if (args.OldItems != null && child is PokerCardNode cardNode && args.OldItems.Contains(cardNode.Card))
                    {
                        cardNode.QueueFree();
                    }
                }
                break;
            case NotifyCollectionChangedAction.Reset:
				
                foreach (var child in GetChildren())
                {
                    child.QueueFree();
                }

                break;
			
        }
    }
}