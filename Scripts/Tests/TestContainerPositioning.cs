using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Tests;


public partial class TestContainerPositioning: Node
{
    [Export] public PackedScene CardPrefab;
    [Export] public CardContainer CardContainer;
    
    public override void _Ready()
    {
        base._Ready();
        CardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<CardNode>() },
            { "contentNodeSize", new Vector2(48, 68) },
            { "separation", 12 },
            { "defaultCardFaceDirection", Enums.CardFace.Up }
        });
        SpawnCardNodeAndAppend();
        GD.Print("task start");
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // GD.Print($"Animate flip called {CardNode.FaceDirection}");
    }

    public void SpawnCardNodeAndAppend()
    {
        var card = new PokerCard(new BaseCardDef()
        {
            BaseCredit = 0,
            Rank = Enums.CardRank.Ace,
            Suit = Enums.CardSuit.Clubs
        });
        var cardNode = CardPrefab.Instantiate<CardNode>();
        AddChild(cardNode);
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", card },
            { "faceDirection", Enums.CardFace.Up },
            { "container", null }
        }); 
        CardContainer.ContentNodes.Add(cardNode);
        GD.Print("done");
    }
}