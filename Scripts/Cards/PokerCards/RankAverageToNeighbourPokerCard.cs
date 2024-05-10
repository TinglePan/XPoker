﻿using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class RankAverageToNeighbourPokerCard: RevealTriggeredPokerCard
{
    public RankAverageToNeighbourPokerCard(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank, BattleEntity owner = null, bool suitAsSecondComparer = false) : base(cardSuit, face, rank, owner, suitAsSecondComparer)
    {
    }

    public override void OnReveal()
    {
        Debug.Assert(Battle != null);
        var index = Node.Container.Cards.IndexOf(this);
        int leftIndex = index - 1, rightIndex = index + 1;
        if (leftIndex >= 0 && leftIndex < Node.Container.Cards.Count && rightIndex >= 0 && rightIndex < Node.Container.Cards.Count)
        {
            if (Node.Container.Cards[leftIndex] is BasePokerCard leftCard && Node.Container.Cards[rightIndex] is BasePokerCard rightCard)
            {
                Rank.Value = (Enums.CardRank)(((int)leftCard.Rank.Value + (int)rightCard.Rank.Value) / 2);
            }
        }
    }
}