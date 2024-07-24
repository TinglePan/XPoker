using Godot;
using XCardGame.Common;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame.Tests;

public partial class TestMouseEventInSubViewport : Control
{
	[Export] public CardNode CardNode;
	public override void _Ready()
	{
		base._Ready();
		var card = new PokerCard(new BaseCardDef
		{
			Rank = Enums.CardRank.Ace,
			Suit = Enums.CardSuit.Clubs
		});
		CardNode.Setup(new CardNode.SetupArgs()
		{
			Content = card,
			FaceDirection = Enums.CardFace.Up,
			HasPhysics = true,
		});
		// GD.Print($"Animate flip called {CardNode.FaceDirection.Value}");
	}
}