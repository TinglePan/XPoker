using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts;

public partial class SceneMgr: Node
{
    private GameMgr _gameMgr;
    private Dictionary<string, IManagedNode> _nodes;
    private List<IManagedNode> _anonymousNodes;

    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        _nodes = new Dictionary<string, IManagedNode>();
        _anonymousNodes = new List<IManagedNode>();
    }

    public T GetNodeById<T>(string nodeIdentifier) where T: Node
    {
        if (!_nodes.ContainsKey(nodeIdentifier))
        {
            return null;
        }
        return _nodes[nodeIdentifier] as T;
    }

    public List<T> GetNodes<T>(string groupName) where T : Node
    {
        bool IsNodeInGroup(IManagedNode node, string gn)
        {
            return node is T typedNode && typedNode.IsInGroup(gn);
        }
        var res = new List<T>();
        foreach (var (_, node) in _nodes)
        {
            if (IsNodeInGroup(node, groupName))
            {
                res.Add((T)node);
            }
        }
        foreach (var node in _anonymousNodes)
        {
            if (IsNodeInGroup(node, groupName))
            {
                res.Add((T)node);
            }
        }
        return res;
    }
    
    public void Register(IManagedNode node)
    {
        if (string.IsNullOrEmpty(node.Identifier))
        {
            if (!_anonymousNodes.Contains(node))
            {
                _anonymousNodes.Add(node);
            }
        } else if (!_nodes.TryAdd(node.Identifier, node))
        {
            GD.PrintErr($"Duplicate node identifier:{node.Identifier} by {node}, existing {_nodes[node.Identifier]}");
        }
    }
}