using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;

namespace XCardGame.Scripts.Ui;

public partial class PlayerTab : Node, ISetup
{
	[Export] public PlayerInfoTab PlayerInfoTab;
	[Export] public Control CardContainer;
	[Export] public PackedScene CardPrefab;
	
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
		foreach (var child in CardContainer.GetChildren())
		{
			child.QueueFree();
		}
		if (Player != null)
		{
			// Player.OnAddHoleCard += OnAddHoleCard;
			// Player.OnRemoveHoleCard += OnRemoveHoleCard;
			Player.HoleCards.CollectionChanged += OnHoleCardChanged;
			PlayerInfoTab.Setup(new Dictionary<string, object> {{"player", Player}});
		}
	}
	
	protected void OnAddHoleCard(PokerPlayer p, BaseCard card)
	{
		var cardNode = CardPrefab.Instantiate<CardNode>();
		cardNode.Setup(new Dictionary<string, object>()
		{
			{ "card", card },
		});
		CardContainer.AddChild(cardNode);
	}

	protected void OnRemoveHoleCard(PokerPlayer p, BaseCard card)
	{
		for (int i = 0; i < CardContainer.GetChildCount(); i++)
		{
			if (CardContainer.GetChild(i) is CardNode cardNode && cardNode.Card == card)
			{
				cardNode.QueueFree();
				break;
			}
		}
	}

	protected void OnHoleCardChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		switch (args.Action)
		{
			case NotifyCollectionChangedAction.Add:
				if (args.NewItems != null)
					foreach (var t in args.NewItems)
					{
						var cardNode = CardPrefab.Instantiate<CardNode>();
						cardNode.Setup(new Dictionary<string, object>()
						{
							{ "card", t },
						});
						CardContainer.AddChild(cardNode);
					}

				break;
			case NotifyCollectionChangedAction.Remove:
				
				foreach (var child in CardContainer.GetChildren())
				{
					if (args.OldItems != null && child is CardNode cardNode && args.OldItems.Contains(cardNode.Card))
					{
						cardNode.QueueFree();
					}
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				
				foreach (var child in CardContainer.GetChildren())
				{
					child.QueueFree();
				}

				break;
			
		}
	}
}