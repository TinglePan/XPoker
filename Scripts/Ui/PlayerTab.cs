using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;

namespace XCardGame.Scripts.Ui;

public partial class PlayerTab : Node, ISetup
{
	[Export] public PlayerInfoTab PlayerInfoTab;
	[Export] public Container HoleCardContainer;
	[Export] public Container SpecialCardContainer;
	[Export] public PackedScene CardPrefab;
	[Export] public PackedScene SpecialCardPrefab;
	
	public PokerPlayer Player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Setup(Dictionary<string, object> args)
	{
		Player = args["player"] as PokerPlayer;
		foreach (var child in HoleCardContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var child in SpecialCardContainer.GetChildren())
		{
			child.QueueFree();
		}
		if (Player != null)
		{
			// Player.OnAddHoleCard += OnAddHoleCard;
			// Player.OnRemoveHoleCard += OnRemoveHoleCard;
			Player.HoleCards.CollectionChanged += (o, eventArgs) => OnCardChanged(HoleCardContainer, eventArgs);
			Player.SpecialCards.CollectionChanged += (o, eventArgs) => OnCardChanged(SpecialCardContainer, eventArgs);
			PlayerInfoTab.Setup(new Dictionary<string, object> {{"player", Player}});
		}
	}

	protected void OnCardChanged(Control targetContainerNode, NotifyCollectionChangedEventArgs args)
	{
		
		switch (args.Action)
		{
			case NotifyCollectionChangedAction.Add:
				if (args.NewItems != null)
					foreach (var t in args.NewItems)
					{
						var cardNode = targetContainerNode == HoleCardContainer ? CardPrefab.Instantiate<CardNode>() : SpecialCardPrefab.Instantiate<SpecialCardNode>();
						targetContainerNode.AddChild(cardNode);
						cardNode.Setup(new Dictionary<string, object>()
						{
							{ "card", t },
						});
					}

				break;
			case NotifyCollectionChangedAction.Remove:
				foreach (var child in targetContainerNode.GetChildren())
				{
					if (args.OldItems != null && child is CardNode cardNode && args.OldItems.Contains(cardNode.Card.Value))
					{
						cardNode.QueueFree();
					}
				}
				break;
			case NotifyCollectionChangedAction.Replace:
				CardNode replacedCardNode = null;
				foreach (var child in targetContainerNode.GetChildren())
				{
					if (args.OldItems != null && child is CardNode cardNode && args.OldItems.Contains(cardNode.Card.Value))
					{
						replacedCardNode = cardNode;
						break;
					}
					// var cardNode = targetContainerNode == HoleCardContainer ? CardPrefab.Instantiate<CardNode>() : SpecialCardPrefab.Instantiate<SpecialCardNode>();
					// targetContainerNode.AddChild(cardNode);
					// cardNode.Setup(new Dictionary<string, object>()
					// {
					// 	{ "card", t },
					// });
				}
				if (replacedCardNode != null && args.NewItems != null)
				{
					replacedCardNode.Card.Value = args.NewItems[0] as BaseCard;
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				foreach (var child in targetContainerNode.GetChildren())
				{
					child.QueueFree();
				}
				break;
			
		}
	}
}