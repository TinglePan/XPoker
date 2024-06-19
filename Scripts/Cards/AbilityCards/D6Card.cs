using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class D6Card: BaseUseCard
{
    public List<CardContainer> CardContainers;
    public D6Card(UseCardDef def): base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
            Battle.Player.HoleCardContainer,
            Battle.Enemy.HoleCardContainer
        };
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
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                if (cardNode.FaceDirection.Value != Enums.CardFace.Up) continue;
                Battle.Dealer.DealCardAndReplace(cardNode, delay:Configuration.AnimateCardTransformInterval * index);
                index++;
            }
        }
    }
}