using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class BaseStackableBuff: BaseBuff, IStackableBuff
{
    public ObservableProperty<int> Stack { get; }
    public int MaxStack { get; }
    
    public BaseStackableBuff(string name, string description, string iconPath, int stack, int maxStack,
        BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard) : 
        base(name, description, iconPath, entity, inflictedBy, inflictedByCard)
    {
        Stack = new ObservableProperty<int>(nameof(Stack), this, stack);
        MaxStack = maxStack;
    }

    public override void Repeat(Battle battle, BattleEntity entity)
    {
        foreach (var buff in entity.Buffs)
        {
            if (buff.Equals(this))
            {
                var stackableBuff = (BaseStackableBuff)buff;
                stackableBuff.Stack.Value = Mathf.Clamp(stackableBuff.Stack.Value + Stack.Value, 0, MaxStack);
                OnStart(battle);
                return;
            }
        }
    }

    public override void OnStart(Battle battle)
    {
        Battle.OnRoundEnd += OnRoundEnd;
    }

    public override void OnStop(Battle battle)
    {
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    protected virtual void OnRoundEnd(Battle battle)
    {
        Stack.Value = Mathf.Clamp(Stack.Value - StackDecreasePerRound(), 0, MaxStack);
        if (Stack.Value <= 0)
        {
            Entity.Buffs.Remove(this);
        }
    }

    protected virtual int StackDecreasePerRound()
    {
        return 1;
    }
}