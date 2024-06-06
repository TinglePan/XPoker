using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;
using BuffNode = XCardGame.Scripts.Nodes.BuffNode;

namespace XCardGame.Scripts.Buffs;

public class BaseBuff:ILifeCycleTriggeredInBattle, ISetup, IContent<BaseBuff>, IEquatable<BaseBuff>
{
    public string Name;
    public string Description;
    public string IconPath;
    public BattleEntity Entity;
    public BattleEntity InflictedBy;
    public BaseCard InflictedByCard;
    
    public HashSet<BaseContentNode<BaseBuff>> Nodes { get; private set; }

    public GameMgr GameMgr;
    public Battle Battle;
    public bool HasSetup { get; set; }

    public BaseBuff(string name, string description, string iconPath, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard)
    {
        Nodes = new HashSet<BaseContentNode<BaseBuff>>();
        Name = name;
        Description = description;
        IconPath = iconPath;
        Entity = entity;
        InflictedBy = inflictedBy;
        InflictedByCard = inflictedByCard;
        HasSetup = false;
    }
    
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode<TContentNode, BaseBuff>
    {
        foreach (var node in Nodes)
        {
            if (node is TContentNode contentNode)
            {
                return contentNode;
            }
        }
        return null;
    }

    public void Setup(Dictionary<string, object> args)
    {
        Nodes.Add((BuffNode)args["node"]);
        GameMgr = (GameMgr)args["gameMgr"];
        Battle = GameMgr.CurrentBattle;
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public virtual void OnStart(Battle battle)
    {
    }

    public virtual void OnStop(Battle battle)
    {
    }

    public virtual void Repeat(Battle battle, BattleEntity entity)
    {
    }

    public void OnDisposal(Battle battle)
    {
        OnStop(battle);
    }

    public bool Equals(BaseBuff other)
    {
        return GetType() == other?.GetType();
    }
}