using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluateRules;

public class StraightGraph
{
    public class StraightGraphNode
    {
        public Enums.CardRank Rank;
        public List<StraightGraphNode> ValidNextNodes;
    }

    public Dictionary<Enums.CardRank, StraightGraphNode> NodeIndex;
    
    public StraightGraph()
    {
        NodeIndex = new Dictionary<Enums.CardRank, StraightGraphNode>();
    }
    
    public void AddNode(Enums.CardRank rank, params Enums.CardRank[] validNextRanks)
    {
        var node = GetNodeOrNew(rank);
        foreach (var nextRank in validNextRanks)
        {
            var nextNode = GetNodeOrNew(nextRank);
            node.ValidNextNodes.Add(nextNode);
        }
    }
    
    public bool IsValidNext(Enums.CardRank currentRank, Enums.CardRank nextRank)
    {
        if (!NodeIndex.ContainsKey(currentRank)) return false;
        var currentNode = NodeIndex[currentRank];
        return currentNode.ValidNextNodes.Any(node => node.Rank == nextRank);
    }
    
    public List<List<Enums.CardRank>> AllPossibleStraightRanges(Enums.CardRank startRank, int steps)
    {
        if (!NodeIndex.TryGetValue(startRank, out var startNode)) return null;
        var res = new List<List<Enums.CardRank>>(); 
        var currentRoute = new List<StraightGraphNode>();
        void Helper(StraightGraphNode currentNode, int stepsLeft)
        {
            if (stepsLeft == 0)
            {
                res.Add(currentRoute.Select(node => node.Rank).ToList());
                return;
            }

            foreach (var nextNode in currentNode.ValidNextNodes)
            {
                currentRoute.Add(nextNode);
                Helper(nextNode, stepsLeft - 1);
                currentRoute.RemoveAt(currentRoute.Count - 1);
            }
        }
        Helper(startNode, steps);
        return res;
    }

    private StraightGraphNode GetNodeOrNew(Enums.CardRank rank)
    {
        if (!NodeIndex.ContainsKey(rank))
        {
            var node = new StraightGraphNode
            {
                Rank = rank, ValidNextNodes = new List<StraightGraphNode>()
            };
            NodeIndex[rank] = node;
        }
        return NodeIndex[rank];
    }
}