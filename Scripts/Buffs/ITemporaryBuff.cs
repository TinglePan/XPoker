using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Buffs;

public interface ITemporaryBuff
{
    public int Duration { get; }
    public ObservableProperty<int> DurationCounter { get; }
}