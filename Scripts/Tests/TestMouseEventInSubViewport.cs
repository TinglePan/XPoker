using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;

using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.Tests;

public partial class TestMouseEventInSubViewport : Control
{
	[Export] public CardNode CardNode;
	public override void _Ready()
	{
		base._Ready();
		var card = new PokerCard(new PokerCardDef
		{
			Rank = Enums.CardRank.Ace,
			Suit = Enums.CardSuit.Clubs
		});
		CardNode.Setup(new Dictionary<string, object>()
		{
			{ "card", card },
			{ "faceDirection", Enums.CardFace.Up },
			{ "container", null },
			{ "hasPhysics", true }
		});
		GD.Print($"Animate flip called {CardNode.FaceDirection.Value}");
	}
}