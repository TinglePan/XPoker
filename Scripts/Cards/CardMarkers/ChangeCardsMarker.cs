using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.CardMarkers;

public class ChangeCardsMarker: BaseCardMarker
{

    public class BaseChangeCardsMarkerSelector
    {
        
    }
    
    public class BaseChangeCardsMarkerFunc
    {
        
    }
    
    public static IEnumerable<PokerCard> SelectLeftNeighbour(Battle battle, PokerCard card)
    {
        var cardNode = card.Node<CardNode>();
        var neighbourIndex = cardNode.Container.Value.Contents.IndexOf(card) - 1;
        if (neighbourIndex >= 0)
        {
            yield return cardNode.Container.Value.Contents[neighbourIndex] as PokerCard;
        }
    }
    
    public static IEnumerable<PokerCard> SelectRightNeighbour(Battle battle, PokerCard card)
    {
        var cardNode = card.Node<CardNode>();
        var container = cardNode.Container.Value;
        var neighbourIndex = container.Contents.IndexOf(card) + 1;
        if (neighbourIndex < container.Contents.Count)
        {
            yield return container.Contents[neighbourIndex] as PokerCard;
        }
    }
    
    public static IEnumerable<PokerCard> SelectBothNeighbours(Battle battle, PokerCard card)
    {
        var cardNode = card.Node<CardNode>();
        var container = cardNode.Container.Value;
        var index = container.Contents.IndexOf(card);
        var leftIndex = index - 1;
        var rightIndex = index + 1;
        if (leftIndex >= 0 && leftIndex < container.Contents.Count)
        {
            yield return container.Contents[leftIndex] as PokerCard;
        }
        if (rightIndex >= 0 && rightIndex < container.Contents.Count)
        {
            yield return container.Contents[rightIndex] as PokerCard;
        }
    }

    public static IEnumerable<PokerCard> SelectUniqueMostCommonSuitCards(Battle battle, PokerCard card)
    {
        var suitCount = new Dictionary<Enums.CardSuit, int>();
        var cardNode = card.Node<CardNode>();
        var container = cardNode.Container.Value;
        foreach (var cardInContainer in container.Contents)
        {
            if (cardInContainer is PokerCard pokerCard)
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
                foreach (var cardInContainer in container.Contents)
                {
                    if (cardInContainer is PokerCard pokerCard && pokerCard.Suit.Value == uniqueMostCommonSuit)
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
    public Action<List<PokerCard>, PokerCard, Battle> RevertFunc;
    
    public ChangeCardsMarker(string description, string texturePath, PokerCard card,
        Func<Battle, PokerCard, IEnumerable<PokerCard>> cardSelector,
        Action<List<PokerCard>, PokerCard, Battle> changeFunc, Action<List<PokerCard>, PokerCard, Battle> revertFunc) : base(description, texturePath, card)
    {
        CardSelector = cardSelector;
        ChangeFunc = changeFunc;
        RevertFunc = revertFunc;

    }

    public override void OnStart(Battle battle)
    {
        var cards = CardSelector(battle, Card).ToList();
        ChangeFunc(cards, Card, Battle);
    }

    public override void OnStop(Battle battle)
    {
        var cards = CardSelector(battle, Card).ToList();
        RevertFunc(cards, Card, Battle);
    }
}