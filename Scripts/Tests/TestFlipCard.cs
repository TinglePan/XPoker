using Godot;
using XCardGame.Common;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame.Tests;

public partial class TestFlipCard: Node
{
    [Export] public CardNode CardNode;
    public override void _Ready()
    {
        base._Ready();
        var card = new SimpleCard(new CardDef
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
        CardNode.AnimateFlip(Enums.CardFace.Down);
        // GD.Print($"Animate flip called {CardNode.FaceDirection.Value}");
    }
}