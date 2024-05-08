using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Buffs;

public interface IStackableBuff
{
    public ObservableProperty<int> Stack { get; }
    public int MaxStack { get; }
}