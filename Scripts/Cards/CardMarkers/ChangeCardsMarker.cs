using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.CardMarkers;

public class ChangeCardsMarker: BaseCardMarker
{

    public class BaseChangeCardsMarkerSelector
    {
        
    }
    
    public class BaseChangeCardsMarkerFunc
    {
        
    }
    
    
    
    public static IEnumerable<MarkerCard> SelectLeftNeighbour(Battle battle, MarkerCard card)
    {
        var neighbourIndex = card.Node.Container.Contents.IndexOf(card) - 1;
        if (neighbourIndex >= 0)
        {
            yield return card.Node.Container.Contents[neighbourIndex] as MarkerCard;
        }
    }
    
    public static IEnumerable<MarkerCard> SelectRightNeighbour(Battle battle, MarkerCard card)
    {
        var neighbourIndex = card.Node.Container.Contents.IndexOf(card) + 1;
        if (neighbourIndex < card.Node.Container.Contents.Count)
        {
            yield return card.Node.Container.Contents[neighbourIndex] as MarkerCard;
        }
    }
    
    public static IEnumerable<MarkerCard> SelectBothNeighbours(Battle battle, MarkerCard card)
    {
        var index = card.Node.Container.Contents.IndexOf(card);
        var leftIndex = index - 1;
        var rightIndex = index + 1;
        if (leftIndex >= 0 && leftIndex < card.Node.Container.Contents.Count)
        {
            yield return card.Node.Container.Contents[leftIndex] as MarkerCard;
        }
        if (rightIndex >= 0 && rightIndex < card.Node.Container.Contents.Count)
        {
            yield return card.Node.Container.Contents[rightIndex] as MarkerCard;
        }
    }

    public static IEnumerable<MarkerCard> SelectUniqueMostCommonSuitCards(Battle battle, MarkerCard card)
    {
        var suitCount = new Dictionary<Enums.CardSuit, int>();
        foreach (var cardInContainer in card.Node.Container.Contents)
        {
            if (cardInContainer is MarkerCard pokerCard)
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
                foreach (var cardInContainer in card.Node.Container.Contents)
                {
                    if (cardInContainer is MarkerCard pokerCard && pokerCard.Suit.Value == uniqueMostCommonSuit)
                    {
                        yield return pokerCard;
                    }
                }
            }
        }
        
    }

    public static void ChangeTargetRankToSourceRank(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        foreach (var targetCard in targetCards)
        {
            targetCard.Rank.Value = sourceCard.Rank.Value;
        }
    }
    
    public static void ChangeTargetSuitToSourceSuit(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        foreach (var targetCard in targetCards)
        {
            targetCard.Suit.Value = sourceCard.Suit.Value;
        }
    }
    
    public static void ChangeSourceRankToAverageOfSelectedCards(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        sourceCard.Rank.Value = (Enums.CardRank)(targetCards.Select(x => (int)x.Rank.Value).Sum() / targetCards.Count);
    }
    
    public static void ChangeSourceRankToAverageOfTwoOrMoreSelectedCards(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        if (targetCards.Count >= 2)
        {
            sourceCard.Rank.Value = (Enums.CardRank)(targetCards.Select(x => (int)x.Rank.Value).Sum() / targetCards.Count);
        }
    }
    
    public static void ChangeSourceRankToTargetRank(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        sourceCard.Rank.Value = targetCards.First().Rank.Value;
    }
    
    public static void ChangeSourceSuitToTargetSuit(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
    {
        sourceCard.Suit.Value = targetCards.First().Suit.Value;
    }
    
    public static void ChangeSourceRankToTargetRankPlusOne(List<MarkerCard> targetCards, MarkerCard sourceCard, Battle battle)
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
    
    public Func<Battle, MarkerCard, IEnumerable<MarkerCard>> CardSelector;
    public Action<List<MarkerCard>, MarkerCard, Battle> ChangeFunc;
    
    public ChangeCardsMarker(string description, string texturePath, MarkerCard card,
        Func<Battle, MarkerCard, IEnumerable<MarkerCard>> cardSelector,
        Action<List<MarkerCard>, MarkerCard, Battle> changeFunc) : base(description, texturePath, card)
    {
        CardSelector = cardSelector;
        ChangeFunc = changeFunc;
    }

    public override void OnStart(Battle battle)
    {
        var cards = CardSelector(battle, Card).ToList();
        ChangeFunc(cards, Card, Battle);
    }
}