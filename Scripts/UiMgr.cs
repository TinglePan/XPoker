using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts;

public partial class UiMgr: Node
{
    private GameMgr _gameMgr;
    private Dictionary<string, IManagedUi> _nodes;
    private List<IManagedUi> _anonymousNodes;

    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        _nodes = new Dictionary<string, IManagedUi>();
        _anonymousNodes = new List<IManagedUi>();
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
        bool IsNodeInGroup(IManagedUi node, string gn)
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
    
    public void Register(IManagedUi node)
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

    public void OpenCommunityCardContainer(ObservableCollection<BaseCard> communityCards)
    {
        var containerNode = GetNodeById<CardContainer>("communityCardContainer");
        containerNode.Setup(new Dictionary<string, object>()
        {
            { "cards", communityCards }
        });
    }

    public void OpenBattleEntityUiCollection(BattleEntity entity)
    {
        var uiCollection = entity == _gameMgr.CurrentBattle.Player ? GetNodeById<PlayerBattleEntity>("playerUiCollection") : GetNodeById<BattleEntity>("enemyUiCollection");
        uiCollection.Setup(new Dictionary<string, object>()
        {
            { "entity", entity },
        });
    }

    public void OpenBattleUiCollection(PlayerBattleEntity player)
    {
        var battleUiCollection = GetNodeById<Battle>("battleUiCollection");
        battleUiCollection.Setup(new Dictionary<string, object>()
        {
            { "player", player }
        });
    }

    public void OpenAbilityCardUi(ObservableCollection<BaseCard> abilityCards)
    {
        var abilityCardContainer = GetNodeById<CardContainer>("abilityCardContainer");
        abilityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", abilityCards }
        });
    }
}