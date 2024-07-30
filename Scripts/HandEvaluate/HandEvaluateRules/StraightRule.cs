using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame;

public class StraightRule: BaseHandEvaluateRule
{
    protected int CardCount;
    protected StraightGraph Graph;
    
    public override Enums.HandTier Tier => Enums.HandTier.Straight;
    
    public StraightRule(int cardCount, bool allowWrap, bool allowShort)
    {
        CardCount = cardCount;
        Graph = StraightGraph.StandardStraightGraph(allowWrap, allowShort);
    }
    
    protected override List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        if (cards.Count < CardCount) return null;
        List<List<BaseCard>> picks = new List<List<BaseCard>>();
        List<BaseCard> currPick = new List<BaseCard>();
        var cardsByRank = new Dictionary<Enums.CardRank, List<BaseCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank.Value)) cardsByRank[card.Rank.Value] = new List<BaseCard>();
            cardsByRank[card.Rank.Value].Add(card);
        }
        
        void Helper(Enums.CardRank rank)
        {
            if (currPick.Count == CardCount)
            {
                var pick = new List<BaseCard>(currPick);
                picks.Add(pick);
                return;
            }
            if (!Graph.NodeIndex.TryGetValue(rank, out var value)) return;
            foreach (var nextNode in value.AllPossibleStraightNextNodes(currPick.Count == 0 || currPick.Count == CardCount - 2))
            {
                if (!cardsByRank.TryGetValue(nextNode.Rank, out var cardsOfRank)) continue;
                foreach (var card in cardsOfRank)
                {
                    currPick.Add(card);
                    Helper(nextNode.Rank);
                    currPick.Remove(card);
                }
            }
        }

        foreach (var rank in Graph.NodeIndex.Keys)
        {
            Helper(rank);
        }

        return picks;
    }

    protected override List<BaseCard> GetPrimaryComparerCards(List<BaseCard> pick, List<BaseCard> cards)
    {
        var pickCardByRank = pick.ToDictionary(card => card.Rank.Value);
        var successorRankHashSet = new HashSet<Enums.CardRank>();
        int i = 0;
        foreach (var card in pick)
        {
            var rank = card.Rank.Value;
            foreach (var nextRank in Graph.AllPossibleStraightNext(rank, i == 0 || i == pick.Count - 2))
            {
                if (pickCardByRank.ContainsKey(nextRank))
                {
                    successorRankHashSet.Add(nextRank);
                }
            }
            i++;
        }

        BaseCard startCard = null;
        foreach (var card in pick)
        {
            if (successorRankHashSet.Contains(card.Rank.Value)) continue;
            startCard = card;
            break;
        }

        if (startCard == null)
        {
            GD.PrintErr("No primary comparer card found. Not supposed to happen.");
            return null;
        }

        // Get second card in straight as primary comparer.
        foreach (var nextRank in Graph.AllPossibleStraightNext(startCard.Rank.Value))
        {
            if (pickCardByRank.TryGetValue(nextRank, out var value))
            {
                return new List<BaseCard> { value };
            }
        }
        return null;
    }
}