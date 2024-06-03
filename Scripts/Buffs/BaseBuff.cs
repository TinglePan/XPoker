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

public class BaseBuff:ILifeCycleTriggeredInBattle, ISetup, IContent<BuffNode, BaseBuff>, IEquatable<BaseBuff>
{
    public BuffNode Node { get; set; }
    public GameMgr GameMgr;
    public Battle Battle;
    public BattleEntity Entity;

    public BattleEntity InflictedBy;
    public BaseCard InflictedByCard;
    
    public bool HasSetup { get; set; }
    
    public string Name;
    public string Description;
    public string IconPath;

    public BaseBuff(string name, string description, string iconPath, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard)
    {
        HasSetup = false;
        Name = name;
        Description = description;
        IconPath = iconPath;
        Entity = entity;
        InflictedBy = inflictedBy;
        InflictedByCard = inflictedByCard;
    }

    public void Setup(Dictionary<string, object> args)
    {
        Node = (BuffNode)args["node"];
        GameMgr = (GameMgr)args["gameMgr"];
        Battle = (Battle)args["battle"];
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