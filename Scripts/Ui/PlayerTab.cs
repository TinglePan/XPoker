using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;

namespace XCardGame.Scripts.Ui;

public partial class PlayerTab : Node, ISetup
{
	[Export] public PlayerInfoTab PlayerInfoTab;
	[Export] public CardContainer HoleCardContainer;
	[Export] public CardContainer SpecialCardContainer;
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
		if (Player != null)
		{
			// Player.OnAddHoleCard += OnAddHoleCard;
			// Player.OnRemoveHoleCard += OnRemoveHoleCard;
			HoleCardContainer.Setup(new Dictionary<string, object>()
			{
				{ "cards", Player.HoleCards }
			});
			SpecialCardContainer.Setup(new Dictionary<string, object>()
			{
				{ "cards", Player.SpecialCards }
			});
			PlayerInfoTab.Setup(new Dictionary<string, object> {{"player", Player}});
		}
	}
}