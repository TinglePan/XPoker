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
    private Dictionary<string, Node> _nodes;
    private List<Node> _anonymousNodes;

    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        _nodes = new Dictionary<string, Node>();
        _anonymousNodes = new List<Node>();
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
        bool IsNodeInGroup(Node node, string gn)
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
    
    public void Register(ManagedBySceneMgrComp comp, Node managedNode)
    {
        if (string.IsNullOrEmpty(comp.Identifier))
        {
            if (!_anonymousNodes.Contains(managedNode))
            {
                _anonymousNodes.Add(managedNode);
            }
        } else if (!_nodes.TryAdd(comp.Identifier, managedNode))
        {
            GD.PrintErr($"Duplicate node identifier:{comp.Identifier} by {managedNode}, existing {_nodes[comp.Identifier]}");
        }
    }
}