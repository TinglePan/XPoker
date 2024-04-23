using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

namespace XCardGame.Scripts.HandEvaluate;

public class DrawingHandEvaluator: BaseHandEvaluator
{
    public int TargetCommunityCardCount;
    public Deck Deck;
    // public Dictionary<string, object> Context;

    public DrawingHandEvaluator(List<BasePokerCard> communityCards, int targetCommunityCardCount, Deck deck, int cardCount,
        int requiredHoleCardCountMin, int requiredHoleCardCountMax, List<BaseHandEvaluateRule> rules = null): 
        base(communityCards, cardCount, requiredHoleCardCountMin, requiredHoleCardCountMax, rules)
    {
        Rules = rules ?? FiveCardHRules;
        CommunityCards = communityCards;
        TargetCommunityCardCount = targetCommunityCardCount;
        Deck = deck;
        CardCount = cardCount;
        RequiredHoleCardCountMin = requiredHoleCardCountMin;
        RequiredHoleCardCountMax = requiredHoleCardCountMax;
    }

    public override List<float> EvaluateOdds(List<List<BasePokerCard>> holeCards)
    {
        throw new NotImplementedException();
    }

    public override float EvaluateAverageOdd(List<BasePokerCard> holeCards, int nTrials)
    {
        var excludedCards = holeCards.Concat(CommunityCards).ToList();
        var dealingDeck = Deck.CreateDealingDeck(excludedCards, true);
        var dealCommunityCardCount = TargetCommunityCardCount - CommunityCards.Count;
        var tempCommunityCards = CommunityCards.ToList();
        var opponentHoleCards = new List<BasePokerCard>();
        var evaluator = new CompletedHandEvaluator(tempCommunityCards, CardCount, RequiredHoleCardCountMin, RequiredHoleCardCountMax, Rules);
        var playerWinCount = 0;
        for (var i = 0; i < nTrials; i++)
        {
            for (var j = 0; j < dealCommunityCardCount; j++)
            {
                tempCommunityCards.Add(dealingDeck.Deal());
            }
            for (var k = 0; k < holeCards.Count; k++)
            {
                opponentHoleCards.Add(dealingDeck.Deal());
            }
            var playerBestHand = evaluator.EvaluateBestHand(holeCards);
            evaluator.Clear();
            var opponentBestHand = evaluator.EvaluateBestHand(opponentHoleCards);
            if (playerBestHand.CompareTo(opponentBestHand) > 0)
            {
                playerWinCount++;
            };
            for (var j = 0; j < dealCommunityCardCount; j++)
            {
                tempCommunityCards.RemoveAt(tempCommunityCards.Count - 1);
            }
            opponentHoleCards.Clear();
            dealingDeck.Reset();
        }
        return (float)playerWinCount / nTrials;
    }
}