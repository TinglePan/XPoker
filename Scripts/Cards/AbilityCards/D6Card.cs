using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class D6Card: BaseUseCard
{
    public List<CardContainer> CardContainers;
    public D6Card(Enums.CardSuit suit, Enums.CardRank rank) : 
        base("Dice 6", "Reroll your destiny", "res://Sprites/Cards/d6.png", suit, rank, 1)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.SceneMgr.GetNodes<CardContainer>("markerCardContainer");
    }

    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void Use()
    {
        base.Use();
        var index = 0;
        foreach (var cardContainer in CardContainers)
        {
            foreach (var card in cardContainer.Contents.ToList())
            {
                if (card.Node.FaceDirection.Value != Enums.CardFace.Up) continue;
                Battle.Dealer.DealCardAndReplace(card.Node, delay:Configuration.AnimateCardTransformInterval * index);
                index++;
            }
        }
    }
}