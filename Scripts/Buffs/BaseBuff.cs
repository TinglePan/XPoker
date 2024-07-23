using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Game;

using XCardGame.Scripts.HandEvaluate;

using XCardGame.Scripts.Ui;
using Battle = XCardGame.Scripts.Game.Battle;
using BattleEntity = XCardGame.Scripts.Game.BattleEntity;
using BuffNode = XCardGame.Scripts.Ui.BuffNode;

namespace XCardGame.Scripts.Buffs;

public class BaseBuff:ILifeCycleTriggeredInBattle, IContent, IEquatable<BaseBuff>
{

    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
        public BaseContentNode Node;
    }
    
    public string Name;
    public string DescriptionTemplate;
    public string IconPath;
    
    public BattleEntity Entity;
    public BattleEntity InflictedBy;
    public BaseCard InflictedByCard;

    public bool IsStackable;
    public bool StackOnRepeat;
    public ObservableProperty<int> Stack;
    public int MaxStack;

    public bool IsTemporary;
    
    public HashSet<BaseContentNode> Nodes { get; private set; }

    public GameMgr GameMgr;
    public Battle Battle;

    public BaseBuff(string name, string descriptionTemplate, string iconPath, bool isStackable = false, bool stackOnRepeat = true, int stack = 0, int maxStack = 0, bool isTemporary = false)
    {
        Nodes = new HashSet<BaseContentNode>();
        Name = name;
        DescriptionTemplate = descriptionTemplate;
        IconPath = iconPath;
        IsStackable = isStackable;
        StackOnRepeat = stackOnRepeat;
        Stack = new ObservableProperty<int>(nameof(Stack), this, stack);
        MaxStack = maxStack;
        IsTemporary = isTemporary;
    }
    
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode
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

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        Nodes.Add(args.Node);
        GameMgr = args.GameMgr;
        Battle = args.Battle;
    }

    public override string ToString()
    {
        return Description();
    }

    public void InflictOn(BattleEntity target, BattleEntity source, BaseCard sourceCard)
    {
        Entity = target;
        InflictedBy = source;
        InflictedByCard = sourceCard;
        Entity.BuffContainer.Buffs.Add(this);
    }

    public virtual void OnStart(Battle battle)
    {
        Battle.OnRoundEnd += OnRoundEnd;
    }

    public virtual void OnStop(Battle battle)
    {
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    public virtual void Repeat(Battle battle, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard)
    {
        InflictedBy = inflictedBy;
        InflictedByCard = inflictedByCard;
        if (IsStackable)
        {
            foreach (var buff in entity.BuffContainer.Buffs)
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
        Entity.BuffContainer.Buffs.Remove(this);
    }

    public virtual void OnRoundEnd(Battle battle)
    {
        if (IsTemporary)
        {
            Entity.BuffContainer.Buffs.Remove(this);
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

    public virtual string Description()
    {
        return DescriptionTemplate;
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
            Entity.BuffContainer.Buffs.Remove(this);
        }
    }
}