using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BaseBuff:IContent, IRoundEnd, IStartStopEffect
{

    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
        public BuffNode Node;
        public BattleEntity Entity;
        public BattleEntity InflictedBy;
        public BaseCard InflictedByCard;
    }
    
    public bool IsEffectActive => Entity?.BuffContainer.Contents.Contains(this) ?? false;
    
    public string Name;
    public string DescriptionTemplate;
    public string IconPath;
    
    public BattleEntity Entity;
    public BattleEntity InflictedBy;
    public BaseCard InflictedByCard;

    public bool IsStackable;
    public bool ConsumeAllStack;
    public bool StackOnRepeat;
    public bool RecastOnRepeat;
    public ObservableProperty<int> Stack;
    public int MaxStack;

    public bool IsTemporary;
    
    public HashSet<BaseContentNode> Nodes { get; }

    public GameMgr GameMgr;
    public Battle Battle;

    public BaseBuff(string name, string descriptionTemplate, string iconPath, bool isStackable = false,
        bool consumeAllStack = true, bool recastOnRepeat = true, bool stackOnRepeat = true, int stack = 0,
        int maxStack = 0, bool isTemporary = false)
    {
        Nodes = new HashSet<BaseContentNode>();
        Name = name;
        DescriptionTemplate = descriptionTemplate;
        IconPath = iconPath;
        IsStackable = isStackable;
        ConsumeAllStack = consumeAllStack;
        RecastOnRepeat = recastOnRepeat;
        StackOnRepeat = stackOnRepeat;
        Stack = new ObservableProperty<int>(nameof(Stack), this, stack);
        MaxStack = maxStack;
        IsTemporary = isTemporary;
    }
    
    public TContentNode Node<TContentNode>(bool strict = true) where TContentNode : BaseContentNode
    {
        foreach (var node in Nodes)
        {
            if (strict)
            {
                if (node is TContentNode contentNode)
                {
                    return contentNode;
                }
            }
            else
            {
                if (node.GetType().IsAssignableTo(typeof(TContentNode)))
                {
                    return (TContentNode)node;
                }
            }
        }
        return null;
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        GameMgr = args.GameMgr;
        Battle = args.Battle;
        if (args.Node != null)
        {
            Nodes.Add(args.Node);
        }
        Entity = args.Entity;
        InflictedBy = args.InflictedBy;
        InflictedByCard = args.InflictedByCard;
    }

    public override string ToString()
    {
        return Description();
    }
    
    public void OnStartEffect()
    {
        Entity.AddBuff(this);
        Effect();
    }
    
    public void OnStopEffect()
    {
        Entity.RemoveBuff(this);
    }

    public virtual void OnRoundEnd()
    {
        if (IsTemporary)
        {
            OnStopEffect();
            return;
        }
        if (IsStackable)
        {
            ChangeStack(-StackDecreasePerRound());
        }
    }

    public virtual void Effect()
    {
        
    }
    
    public void RepeatOn(BaseBuff existingBuff)
    {
        if (IsStackable)
        {
            if (StackOnRepeat)
            {
                existingBuff.ChangeStack(Stack.Value);
            }
            else
            {
                existingBuff.Stack.Value = Mathf.Max(existingBuff.Stack.Value, Stack.Value);
            }
        }
        if (RecastOnRepeat)
        {
            existingBuff.Effect();
        }
    }

    public virtual void Consume()
    {
        if (IsStackable && !ConsumeAllStack)
        {
            ChangeStack(-1);
        }
        else
        {
            OnStopEffect();
        }
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
            Stack.Value = Mathf.Clamp(Stack.Value + amount, 0, MaxStack);
        }
        if (Stack.Value <= 0)
        {
            OnStopEffect();
        }
    }
}