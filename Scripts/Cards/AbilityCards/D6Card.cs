using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class D6Card: BaseActivatableAbilityCard, IActivatableCard
{
    private List<CardContainer> _cardContainers;
    public D6Card(GameMgr gameMgr, Enums.CardFace face, Enums.CardSuit suit, GameLogic.BattleEntity owner) : 
        base(gameMgr, "Dice 6", "Reroll your destiny", face, suit,
        "res://Sprites/Cards/d6.png", 1, 0, false, owner)
    {
        _cardContainers = gameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }

    void IActivatableCard.Activate()
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