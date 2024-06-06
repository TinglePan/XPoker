using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Tests;

public partial class TestMouseEventInSubViewport : Control
{
	[Export] public CardNode CardNode;
	public override void _Ready()
	{
		base._Ready();
		var card = new MarkerCard(Utils.GetCardTexturePath(Enums.CardSuit.Clubs), Enums.CardSuit.Clubs, Enums.CardRank.Ace);
		CardNode.Setup(new Dictionary<string, object>()
		{
			{ "card", card },
			{ "faceDirection", Enums.CardFace.Up },
			{ "container", null }
		});
		GD.Print($"Animate flip called {CardNode.FaceDirection.Value}");
	}
}