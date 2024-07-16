using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;

using CardContainer = XCardGame.Scripts.Ui.CardContainer;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.Tests;


public partial class TestContainerPositioning: Node
{
    [Export] public PackedScene CardPrefab;
    [Export] public CardContainer CardContainer;
    
    public override void _Ready()
    {
        base._Ready();
        CardContainer.Setup(new CardContainer.SetupArgs
        {
            ContentNodeSize = new Vector2(48, 68),
            Separation = new Vector2(12, 12),
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            DefaultCardFaceDirection = Enums.CardFace.Up,
            Margins = Configuration.DefaultContentContainerMargins,
        });
        SpawnCardNodeAndAppend();
        GD.Print("task start");
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // GD.Print($"Animate flip called {CardNode.FaceDirection}");
    }

    public void SpawnCardNodeAndAppend()
    {
        var card = new PokerCard(new PokerCardDef
        {
            Rank = Enums.CardRank.Ace,
            Suit = Enums.CardSuit.Clubs
        });
        var cardNode = CardPrefab.Instantiate<CardNode>();
        AddChild(cardNode);
        cardNode.Setup(new CardNode.SetupArgs
        {
            Content = card,
            FaceDirection = Enums.CardFace.Up,
            HasPhysics = true,
        }); 
        CardContainer.ContentNodes.Add(cardNode);
        GD.Print("done");
    }
}