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
        
        SplitCardContainer.Setup(new SplitCardContainer.SetupArgs()
        {
            CardContainersSetupArgs = new List<CardContainer.SetupArgs>
            {
                new ()
                {
                    ContentNodeSize = Configuration.CardSize,
                    Separation = Configuration.CardContainerSeparation,
                    PivotDirection = Enums.Direction2D8Ways.Neutral,
                    DefaultCardFaceDirection = Enums.CardFace.Up,
                    Margins = Configuration.DefaultContentContainerMargins,
                },
                new ()
                {
                    ContentNodeSize = Configuration.CardSize,
                    Separation = Configuration.CardContainerSeparation,
                    PivotDirection = Enums.Direction2D8Ways.Neutral,
                    DefaultCardFaceDirection = Enums.CardFace.Up,
                    Margins = Configuration.DefaultContentContainerMargins,
                },
            },
            Separation = Configuration.SplitCardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
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
            var card = new PokerCard(new BaseCardDef
            {
                Rank = Enums.CardRank.Ace,
                Suit = Enums.CardSuit.Clubs
            });
            var cardNode = Utils.InstantiatePrefab(CardPrefab, SplitCardContainer.CardContainers[0]) as CardNode;
            AddChild(cardNode);
            cardNode?.Setup(new CardNode.SetupArgs()
            {
                Content = card,
                FaceDirection = Enums.CardFace.Up,
                Container = SplitCardContainer.CardContainers[0],
                HasPhysics = true,
            });
            SplitCardContainer.CardContainers[0].ContentNodes.Add(cardNode);
        }
        for (int i = 0; i < 2; i++)
        {
            var card = new PokerCard(new BaseCardDef
            {
                Rank = Enums.CardRank.Ace,
                Suit = Enums.CardSuit.Clubs
            });
            var cardNode = Utils.InstantiatePrefab(CardPrefab, SplitCardContainer.CardContainers[0]) as CardNode;
            AddChild(cardNode);
            cardNode?.Setup(new CardNode.SetupArgs()
            {
                Content = card,
                FaceDirection = Enums.CardFace.Up,
                Container = SplitCardContainer.CardContainers[1],
                HasPhysics = true,
            });
            SplitCardContainer.CardContainers[1].ContentNodes.Add(cardNode);
        }
        GD.Print("done");
    }
}