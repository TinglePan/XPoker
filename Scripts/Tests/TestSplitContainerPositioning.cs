using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Tests;

public partial class TestSplitContainerPositioning: Node
{
    
    [Export] public PackedScene CardPrefab;
    [Export] public SplitCardContainer SplitCardContainer;
    
    public override void _Ready()
    {
        base._Ready();
        
        SplitCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cardContainersSetupArgs", new List<Dictionary<string, object>>()
            {
                new ()
                {
                    { "allowInteract", false },
                    { "cards", new ObservableCollection<BaseCard>() },
                    { "contentNodeSize", Configuration.CardSize },
                    { "separation", Configuration.CardContainerSeparation },
                    { "pivotDirection", Enums.Direction2D8Ways.Neutral },
                    { "nodesPerRow", 0 },
                    { "hasBorder", false },
                    { "expectedContentNodeCount", Configuration.DefaultHoleCardCount },
                    { "hasName", true },
                    { "containerName", "Primary cards" },
                    { "defaultCardFaceDirection", Enums.CardFace.Up },
                    { "margins", Configuration.DefaultContentContainerMargins },
                    { "withCardEffect", true }
                },
                new ()
                {
                    { "allowInteract", false },
                    { "cards", new ObservableCollection<BaseCard>() },
                    { "contentNodeSize", Configuration.CardSize },
                    { "separation", Configuration.CardContainerSeparation },
                    { "pivotDirection", Enums.Direction2D8Ways.Neutral },
                    { "nodesPerRow", 0 },
                    { "hasBorder", false },
                    { "expectedContentNodeCount", Configuration.DefaultHoleCardCount },
                    { "hasName", true },
                    { "containerName", "Kickers" },
                    { "defaultCardFaceDirection", Enums.CardFace.Up },
                    { "margins", Configuration.DefaultContentContainerMargins },
                    { "withCardEffect", true }
                }
            } },
            { "separation", 24 },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral }
        });
        
        SpawnCardNodeAndAppend();
        GD.Print("task start");
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // GD.Print($"Animate flip called {CardNode.FaceDirection}");
    }

    public void SpawnCardNodeAndAppend()
    {
        for (int i = 0; i < 3; i++)
        {
            var card = new PokerCard(new PokerCardDef
            {
                Rank = Enums.CardRank.Ace,
                Suit = Enums.CardSuit.Clubs
            });
            var cardNode = Utils.InstantiatePrefab(CardPrefab, SplitCardContainer.CardContainers[0]) as CardNode;
            AddChild(cardNode);
            cardNode?.Setup(new Dictionary<string, object>
            {
                { "card", card },
                { "faceDirection", Enums.CardFace.Up },
                { "container", SplitCardContainer.CardContainers[0] },
                { "hasPhysics", true }
            }); 
            SplitCardContainer.CardContainers[0].ContentNodes.Add(cardNode);
        }
        for (int i = 0; i < 2; i++)
        {
            var card = new PokerCard(new PokerCardDef
            {
                Rank = Enums.CardRank.Ace,
                Suit = Enums.CardSuit.Clubs
            });
            var cardNode = Utils.InstantiatePrefab(CardPrefab, SplitCardContainer.CardContainers[0]) as CardNode;
            AddChild(cardNode);
            cardNode?.Setup(new Dictionary<string, object>
            {
                { "card", card },
                { "faceDirection", Enums.CardFace.Up },
                { "container", SplitCardContainer.CardContainers[1] },
                { "hasPhysics", true }
            }); 
            SplitCardContainer.CardContainers[1].ContentNodes.Add(cardNode);
        }
        GD.Print("done");
    }
}