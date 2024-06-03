using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class BaseTemporaryBuff: BaseBuff, ITemporaryBuff
{
    public int Duration { get; private set; }
    public ObservableProperty<int> DurationCounter { get; private init; }
    public BaseTemporaryBuff(string name, string description, string iconPath, int duration, BattleEntity entity, BattleEntity inflictedBy,
        BaseCard inflictedByCard) : base(name, description, iconPath, entity, inflictedBy, inflictedByCard)
    {
        Duration = duration;
        DurationCounter = new ObservableProperty<int>(nameof(DurationCounter), this, 0);
    }

    public override void Repeat(Battle battle, BattleEntity entity)
    {
        OnStart(battle);
    }

    public override void OnStart(Battle battle)
    {
        DurationCounter.Value = Duration;
        Battle.OnRoundEnd += OnRoundEnd;
    }

    public override void OnStop(Battle battle)
    {
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    protected virtual void OnRoundEnd(Battle battle)
    {
        DurationCounter.Value--;
        if (DurationCounter.Value <= Duration)
        {
            Entity.BuffContainer.Contents.Remove(this);
        }
    }

}