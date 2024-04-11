using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public partial class Test : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var playerHoldCards = new List<BaseCard>()
		{
			new NumericCard(Enums.CardSuit.Diamonds, Enums.CardRank.Ace, Enums.CardFace.Down),
			new NumericCard(Enums.CardSuit.Clubs, Enums.CardRank.King, Enums.CardFace.Down)
		};
		var communityCards = new List<BaseCard>()
		{
			new NumericCard(Enums.CardSuit.Spades, Enums.CardRank.Seven, Enums.CardFace.Up),
			new NumericCard(Enums.CardSuit.Clubs, Enums.CardRank.Two, Enums.CardFace.Up),
			new NumericCard(Enums.CardSuit.Clubs, Enums.CardRank.Six, Enums.CardFace.Up),
			new NumericCard(Enums.CardSuit.Hearts, Enums.CardRank.Four, Enums.CardFace.Up),
			new NumericCard(Enums.CardSuit.Clubs, Enums.CardRank.Three, Enums.CardFace.Up),
		};
		var evaluator = new HandEvaluator(playerHoldCards, communityCards, 5, 0,
			2);
		var bestHand = evaluator.EvaluateBestHand();
		GD.Print($"Best Hand: {bestHand.Rank}, {string.Join(",", bestHand.PrimaryCards)} / {string.Join(",", bestHand.Kickers)}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}