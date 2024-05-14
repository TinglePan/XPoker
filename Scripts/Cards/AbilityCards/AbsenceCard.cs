using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class AbsenceCard: BasePassiveCard
{
    public Func<PokerCard, bool> Filter;
    public List<CardContainer> CardContainers;
    public List<PokerCard> NegatedCards;
    
    public AbsenceCard(string name, string description, string iconPath, Enums.CardFace face, Enums.CardSuit suit,
        Enums.CardRank rank, int cost, int sealDuration, Func<PokerCard, bool> filter, bool isQuick = true, BattleEntity owner = null) : 
        base(name, description, iconPath, face, suit, rank, cost, sealDuration, isQuick, owner)
    {
        Filter = filter;
        NegatedCards = new List<PokerCard>();
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }

    public override void OnAppear(Battle battle)
    {
        bool hasNegatedCards = false;
        foreach (var cardContainer in CardContainers)
        {
            foreach (var baseCard in cardContainer.Cards)
            {
                var card = (PokerCard)baseCard;
                if (Filter(card) && !card.IsNegated.Value)
                {
                    card.IsNegated.Value = true;
                    NegatedCards.Add(card);
                    hasNegatedCards = true;
                }
            }
        }
        if (hasNegatedCards)
        {
            AfterEffect();
        }
    }

    public override void OnDisappear(Battle battle)
    {
        foreach (var card in NegatedCards)
        {
            card.IsNegated.Value = true;
        }
    }
}