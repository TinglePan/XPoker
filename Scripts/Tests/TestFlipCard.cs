using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Tests;

public partial class TestFlipCard: Node
{
    [Export] public CardNode CardNode;
    public override void _Ready()
    {
        base._Ready();
        var card = new PokerCard(new BaseCardDef()
        {
            BasePrice = 0,
            Rank = Enums.CardRank.Ace,
            Suit = Enums.CardSuit.Clubs
        });
        CardNode.Setup(new Dictionary<string, object>()
        {
            { "card", card },
            { "faceDirection", Enums.CardFace.Up },
            { "container", null }
        });
        CardNode.AnimateFlip(Enums.CardFace.Down);
        GD.Print($"Animate flip called {CardNode.FaceDirection.Value}");
    }
}