using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class StraightRule: BaseHandEvaluateRule
{
    protected int CardCount;
    protected bool CanWrap;
    protected bool AllowAceLowStraight;
    protected List<Enums.CardRank> Range;
    
    public override Enums.HandTier Tier => Enums.HandTier.Straight;
    
    public static List<Enums.CardRank> DefaultRange = new()
    {
        Enums.CardRank.Two,
        Enums.CardRank.Three,
        Enums.CardRank.Four,
        Enums.CardRank.Five,
        Enums.CardRank.Six,
        Enums.CardRank.Seven,
        Enums.CardRank.Eight,
        Enums.CardRank.Nine,
        Enums.CardRank.Ten,
        Enums.CardRank.Jack,
        Enums.CardRank.Queen,
        Enums.CardRank.King,
        Enums.CardRank.Ace,
    };
    
    public StraightRule(int cardCount, bool canWrap, bool allowAceLowStraight)
    {
        CardCount = cardCount;
        CanWrap = canWrap;
        Range = new List<Enums.CardRank>(DefaultRange);
        AllowAceLowStraight = canWrap || allowAceLowStraight;
    }
    
    protected override List<List<PokerCard>> Pick(List<PokerCard> cards)
    {
        List<List<PokerCard>> picks = new List<List<PokerCard>>();
        List<PokerCard> currPick = new List<PokerCard>();
        var cardsByRank = new Dictionary<Enums.CardRank, List<PokerCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank.Value)) cardsByRank[card.Rank.Value] = new List<PokerCard>();
            cardsByRank[card.Rank.Value].Add(card);
        }
        
        void Helper(int currentRankIndexInRange)
        {
            if (currPick.Count == CardCount)
            {
                picks.Add(new List<PokerCard>(currPick));
                return;
            }
            var currRank = Range[currentRankIndexInRange];
            var nextRankIndex = (currentRankIndexInRange + 1) % Range.Count;
            var cardsOfRank = cardsByRank[currRank];
            foreach (var card in cardsOfRank)
            {
                currPick.Add(card);
                Helper(nextRankIndex);
                currPick.Remove(card);
            }
        }

        for (int i = 0; i < Range.Count; i++)
        {
            if (i + CardCount > Range.Count && !CanWrap && !(AllowAceLowStraight && Range[i] == Enums.CardRank.Ace))
            {
                continue;
            }

            var haveStraightAtRank = true;
            for (int j = 0; j < CardCount; j++)
            {
                var searchRank = Range[(i + j) % Range.Count];
                if (!cardsByRank.ContainsKey(searchRank))
                {
                    haveStraightAtRank = false;
                    break;
                }
            }
            if (haveStraightAtRank)
            {
                Helper(i);
            }
        }

        return picks;
    }

    protected override List<PokerCard> GetPrimaryComparerCards(List<PokerCard> pick, List<PokerCard> cards)
    {
        if (!AllowAceLowStraight && !CanWrap) return base.GetPrimaryComparerCards(pick, cards);
        var pickCardByRank = pick.ToDictionary(card => card.Rank.Value);
        PokerCard endAtPokerCard = null;
        for (int i = 0; i < Range.Count; i++)
        {
            bool found = true;
            for (int j = 0; j < CardCount; j++)
            {
                var searchRank = Range[(i + j) % Range.Count];
                if (!pickCardByRank.ContainsKey(searchRank))
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                endAtPokerCard = pickCardByRank[Range[(i + CardCount - 1) % Range.Count]];
            } 
        }
        return new List<PokerCard>() { endAtPokerCard };
    }
}