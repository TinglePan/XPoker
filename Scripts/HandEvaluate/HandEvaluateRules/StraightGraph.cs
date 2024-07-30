using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame;

public class StraightGraph
{
    public class StraightGraphNode
    {
        public Enums.CardRank Rank;
        public bool IsTerminal;
        public List<StraightGraphNode> ValidNextNodes;
        
        public List<StraightGraphNode> AllPossibleStraightNextNodes(bool isTerminal = false)
        {
            return ValidNextNodes.Where(node => !node.IsTerminal || isTerminal).ToList();
        }
    }

    public static StraightGraph StandardStraightGraph(bool allowWrap = false, bool allowShort = false)
    {
        var res = new StraightGraph();
        var aceNode = res.GetNodeOrNew(Enums.CardRank.Ace);
        if (!allowWrap)
        {
            aceNode.IsTerminal = true;
        }
        res.GetNodeOrNew(Enums.CardRank.Two);
        res.GetNodeOrNew(Enums.CardRank.Three);
        res.GetNodeOrNew(Enums.CardRank.Four);
        res.GetNodeOrNew(Enums.CardRank.Five);
        res.GetNodeOrNew(Enums.CardRank.Six);
        res.GetNodeOrNew(Enums.CardRank.Seven);
        res.GetNodeOrNew(Enums.CardRank.Eight);
        res.GetNodeOrNew(Enums.CardRank.Nine);
        res.GetNodeOrNew(Enums.CardRank.Ten);
        res.GetNodeOrNew(Enums.CardRank.Jack);
        res.GetNodeOrNew(Enums.CardRank.Queen);
        res.GetNodeOrNew(Enums.CardRank.King);
        
        res.AddEdge(Enums.CardRank.Ace, Enums.CardRank.Two);
        res.AddEdge(Enums.CardRank.Two, Enums.CardRank.Three);
        res.AddEdge(Enums.CardRank.Three, Enums.CardRank.Four);
        res.AddEdge(Enums.CardRank.Four, Enums.CardRank.Five);
        res.AddEdge(Enums.CardRank.Five, Enums.CardRank.Six);
        res.AddEdge(Enums.CardRank.Six, Enums.CardRank.Seven);
        res.AddEdge(Enums.CardRank.Seven, Enums.CardRank.Eight);
        res.AddEdge(Enums.CardRank.Eight, Enums.CardRank.Nine);
        res.AddEdge(Enums.CardRank.Nine, Enums.CardRank.Ten);
        res.AddEdge(Enums.CardRank.Ten, Enums.CardRank.Jack);
        res.AddEdge(Enums.CardRank.Jack, Enums.CardRank.Queen);
        res.AddEdge(Enums.CardRank.Queen, Enums.CardRank.King);
        res.AddEdge(Enums.CardRank.King, Enums.CardRank.Ace);
        
        if (allowShort)
        {
            res.AddEdge(Enums.CardRank.Ace, Enums.CardRank.Six);
        }
        return res;
    }

    public Dictionary<Enums.CardRank, StraightGraphNode> NodeIndex;
    
    public StraightGraph()
    {
        NodeIndex = new Dictionary<Enums.CardRank, StraightGraphNode>();
    }

    public void AddEdge(Enums.CardRank rank, params Enums.CardRank[] validNextRanks)
    {
        var node = GetNodeOrNew(rank);
        foreach (var nextRank in validNextRanks)
        {
            var nextNode = GetNodeOrNew(nextRank);
            node.ValidNextNodes.Add(nextNode);
        }
    }
    
    public bool IsValidNext(Enums.CardRank currentRank, Enums.CardRank nextRank, bool isTerminal = false)
    {
        if (!NodeIndex.ContainsKey(currentRank)) return false;
        var currentNode = NodeIndex[currentRank];
        return currentNode.ValidNextNodes.Any(node => node.Rank == nextRank && (!node.IsTerminal || isTerminal));
    }
    
    public List<Enums.CardRank> AllPossibleStraightNext(Enums.CardRank startRank, bool isTerminal = false)
    {
        if (!NodeIndex.TryGetValue(startRank, out var startNode)) return null;
        return startNode.ValidNextNodes.Where(node => !node.IsTerminal || isTerminal).Select(node => node.Rank).ToList();
    }
    
    public List<List<Enums.CardRank>> AllPossibleStraightSequences(Enums.CardRank startRank, int steps, bool isTerminal = false)
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
                if (!isTerminal && nextNode.IsTerminal) continue;
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
                Rank = rank, IsTerminal = false, ValidNextNodes = new List<StraightGraphNode>()
            };
            NodeIndex[rank] = node;
        }
        return NodeIndex[rank];
    }
}