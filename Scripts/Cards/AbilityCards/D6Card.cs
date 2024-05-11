using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class D6Card: BaseActivatableCard
{
    private List<CardContainer> _cardContainers;
    public D6Card(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1, int coolDown = 0,
        bool isQuick = false, BattleEntity owner = null) : 
        base("Dice 6", "Reroll your destiny", "res://Sprites/Cards/d6.png", face, suit, rank, 
            cost, coolDown, isQuick, owner)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        _cardContainers = GameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }

    public override void Activate()
    {
        foreach (var cardContainer in _cardContainers)
        {
            for (var i = 0; i < cardContainer.Cards.Count; i++)
            {
                var card = cardContainer.Cards[i];
                if (card.Face.Value != Enums.CardFace.Up) continue;
                var newCard = GameMgr.CurrentBattle.DealingDeck.Deal();
                newCard.Face.Value = Enums.CardFace.Up;
                cardContainer.Cards[i] = newCard;
            }
        }
        AfterEffect();
    }
}