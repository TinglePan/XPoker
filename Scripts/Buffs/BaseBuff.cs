using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public bool IsStackable;
    public bool StackOnRepeat;
    public ObservableProperty<int> Stack;
    public int MaxStack;

    public bool IsTemporary;
    
    public HashSet<BaseContentNode<BaseBuff>> Nodes { get; private set; }

    public GameMgr GameMgr;
    public Battle Battle;
    public bool HasSetup { get; set; }

    public BaseBuff(string name, string description, string iconPath, bool isStackable = false, bool stackOnRepeat = true, int stack = 0, int maxStack = 0, bool isTemporary = false)
    {
        Nodes = new HashSet<BaseContentNode<BaseBuff>>();
        Name = name;
        Description = description;
        IconPath = iconPath;
        IsStackable = isStackable;
        StackOnRepeat = stackOnRepeat;
        Stack = new ObservableProperty<int>(nameof(Stack), this, stack);
        MaxStack = maxStack;
        IsTemporary = isTemporary;
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

    public void InflictOn(BattleEntity target, BattleEntity source, BaseCard sourceCard)
    {
        EnsureSetup();
        Entity = target;
        InflictedBy = source;
        InflictedByCard = sourceCard;
        Entity.Buffs.Add(this);
    }

    public virtual void OnStart(Battle battle)
    {
        EnsureSetup();
        Battle.OnRoundEnd += OnRoundEnd;
    }

    public virtual void OnStop(Battle battle)
    {
        EnsureSetup();
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    public virtual void Repeat(Battle battle, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard)
    {
        EnsureSetup();
        InflictedBy = inflictedBy;
        InflictedByCard = inflictedByCard;
        if (IsStackable)
        {
            foreach (var buff in entity.Buffs)
            {
                if (buff.Equals(this))
                {
                    if (StackOnRepeat)
                    {
                        buff.ChangeStack(Stack.Value);
                    }
                    else
                    {
                        buff.Stack.Value = Mathf.Max(buff.Stack.Value, Stack.Value);
                    }
                    break;
                }
            }
        }
        OnStart(battle);
    }

    public virtual void Consume()
    {
        EnsureSetup();
        Entity.Buffs.Remove(this);
    }

    public virtual void OnRoundEnd(Battle battle)
    {
        EnsureSetup();
        if (IsTemporary)
        {
            Entity.Buffs.Remove(this);
            return;
        }
        if (IsStackable)
        {
            ChangeStack(-StackDecreasePerRound());
        }
    }

    public bool Equals(BaseBuff other)
    {
        return Name == other?.Name;
    }

    public virtual string GetDescription()
    {
        return Description;
    }
    
    protected virtual int StackDecreasePerRound()
    {
        return 0;
    }

    protected void ChangeStack(int amount)
    {
        if (MaxStack == 0)
        {
            Stack.Value += amount;
        }
        else
        {
            Stack.Value = Mathf.Clamp(Stack.Value + StackDecreasePerRound(), 0, MaxStack);
        }
        if (Stack.Value <= 0)
        {
            Entity.Buffs.Remove(this);
        }
    }
}