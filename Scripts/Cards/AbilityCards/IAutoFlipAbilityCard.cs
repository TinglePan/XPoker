using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Cards.AbilityCards;

public interface IAutoFlipAbilityCard
{
    public bool AutoFlipUp { get; }
    public bool AutoFlipDown { get; }
    public int UpDuration { get; }
    public int DownDuration { get; }
    public ObservableProperty<int> DownDurationCounter { get; }
    public ObservableProperty<int> UpDurationCounter { get; }
}