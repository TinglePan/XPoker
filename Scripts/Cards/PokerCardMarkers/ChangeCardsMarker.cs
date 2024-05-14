using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCardMarkers;

public class ChangeCardsMarker: BasePokerCardMarker
{
    public static IEnumerable<PokerCard> SelectLeftNeighbour(Battle battle, PokerCard card)
    {
        var neighbourIndex = card.Node.Container.Cards.IndexOf(card) - 1;
        if (neighbourIndex >= 0)
        {
            yield return card.Node.Container.Cards[neighbourIndex] as PokerCard;
        }
    }
    
    public static IEnumerable<PokerCard> SelectRightNeighbour(Battle battle, PokerCard card)
    {
        var neighbourIndex = card.Node.Container.Cards.IndexOf(card) + 1;
        if (neighbourIndex < card.Node.Container.Cards.Count)
        {
            yield return card.Node.Container.Cards[neighbourIndex] as PokerCard;
        }
    }
    
    public static IEnumerable<PokerCard> SelectBothNeighbours(Battle battle, PokerCard card)
    {
        var index = card.Node.Container.Cards.IndexOf(card);
        var leftIndex = index - 1;
        var rightIndex = index + 1;
        if (leftIndex >= 0 && leftIndex < card.Node.Container.Cards.Count)
        {
            yield return card.Node.Container.Cards[leftIndex] as PokerCard;
        }
        if (rightIndex >= 0 && rightIndex < card.Node.Container.Cards.Count)
        {
            yield return card.Node.Container.Cards[rightIndex] as PokerCard;
        }
    }

    public static IEnumerable<PokerCard> SelectUniqueMostCommonSuitCards(Battle battle, PokerCard card)
    {
        var suitCount = new Dictionary<Enums.CardSuit, int>();
        foreach (var cardInContainer in card.Node.Container.Cards)
        {
            if (cardInContainer is PokerCard pokerCard && cardInContainer.Face.Value == Enums.CardFace.Up)
            {
                if (!suitCount.TryAdd(pokerCard.Suit.Value, 1))
                {
                    suitCount[pokerCard.Suit.Value]++;
                }
            }
        }
        var sortedSuitCount = suitCount.OrderByDescending(x => x.Value).ToList();
        if (sortedSuitCount.Count > 0)
        {
            if (sortedSuitCount.Count == 1 || sortedSuitCount[0].Value > sortedSuitCount[1].Value)
            {
                var uniqueMostCommonSuit = sortedSuitCount[0].Key;
                foreach (var cardInContainer in card.Node.Container.Cards)
                {
                    if (cardInContainer is PokerCard pokerCard && cardInContainer.Face.Value == Enums.CardFace.Up && pokerCard.Suit.Value == uniqueMostCommonSuit)
                    {
                        yield return pokerCard;
                    }
                }
            }
        }
        
    }

    public static void ChangeTargetRankToSourceRank(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        foreach (var targetCard in targetCards)
        {
            targetCard.Rank.Value = sourceCard.Rank.Value;
        }
    }
    
    public static void ChangeTargetSuitToSourceSuit(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        foreach (var targetCard in targetCards)
        {
            targetCard.Suit.Value = sourceCard.Suit.Value;
        }
    }
    
    public static void ChangeSourceRankToAverageOfSelectedCards(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        sourceCard.Rank.Value = (Enums.CardRank)(targetCards.Select(x => (int)x.Rank.Value).Sum() / targetCards.Count);
    }
    
    public static void ChangeSourceRankToAverageOfTwoOrMoreSelectedCards(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        if (targetCards.Count >= 2)
        {
            sourceCard.Rank.Value = (Enums.CardRank)(targetCards.Select(x => (int)x.Rank.Value).Sum() / targetCards.Count);
        }
    }
    
    public static void ChangeSourceRankToTargetRank(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        sourceCard.Rank.Value = targetCards.First().Rank.Value;
    }
    
    public static void ChangeSourceSuitToTargetSuit(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        sourceCard.Suit.Value = targetCards.First().Suit.Value;
    }
    
    public static void ChangeSourceRankToTargetRankPlusOne(List<PokerCard> targetCards, PokerCard sourceCard, Battle battle)
    {
        var targetRank = targetCards.First().Rank.Value;
        if (targetRank == Enums.CardRank.Ace)
        {
            targetRank = Enums.CardRank.Two;
        }
        else if (targetRank is > Enums.CardRank.Two and < Enums.CardRank.Ace)
        {
            targetRank++;
        }
        {
            targetRank++;
        }
        sourceCard.Rank.Value = targetRank;
    }
    
    public Func<Battle, PokerCard, IEnumerable<PokerCard>> CardSelector;
    public Action<List<PokerCard>, PokerCard, Battle> ChangeFunc;
    
    public ChangeCardsMarker(string description, string texturePath, PokerCard card,
        Func<Battle, PokerCard, IEnumerable<PokerCard>> cardSelector,
        Action<List<PokerCard>, PokerCard, Battle> changeFunc) : base(description, texturePath, card)
    {
        CardSelector = cardSelector;
        ChangeFunc = changeFunc;
    }

    public override void OnAppear(Battle battle)
    {
        var cards = CardSelector(battle, Card).ToList();
        ChangeFunc(cards, Card, Battle);
    }
}