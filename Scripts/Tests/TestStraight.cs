using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame.Tests;

public partial class TestStraight : Node
{
    [Export] public CardNode CardNode;
    public override void _Ready()
    {
        base._Ready();

        var evaluator = new CompletedHandEvaluator(5, 0, 2);
        var communityCards = new List<BaseCard>
        {
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Ace,
                Suit = Enums.CardSuit.Clubs
            }),
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Two,
                Suit = Enums.CardSuit.Clubs
            }),
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Three,
                Suit = Enums.CardSuit.Hearts
            }),
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Seven,
                Suit = Enums.CardSuit.Hearts
            }),
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Ten,
                Suit = Enums.CardSuit.Hearts
            }),
        };
        var holeCards = new List<BaseCard>
        {
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Four,
                Suit = Enums.CardSuit.Clubs
            }),
            new SimpleCard(new CardDef
            {
                Rank = Enums.CardRank.Five,
                Suit = Enums.CardSuit.Clubs
            }),
        };
        var bestHand = evaluator.EvaluateBestHand(communityCards, holeCards, ((Enums.HandTier[])Enum.GetValues(typeof(Enums.HandTier))).Reverse().ToList());
        GD.Print(bestHand.Tier);
    }
}