using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class BaseTemporaryBuff: BaseBuff, ITemporaryBuff
{
    public int Duration { get; private set; }
    public ObservableProperty<int> DurationCounter { get; private init; }
    public BaseTemporaryBuff(string name, string description, string iconPath, GameMgr gameMgr, BattleEntity entity,
        int duration) : base(name, description, iconPath, gameMgr, entity)
    {
        Duration = duration;
        DurationCounter = new ObservableProperty<int>(nameof(DurationCounter), this, 0);
        Battle.OnRoundEnd += OnRoundEnd;
    }
    
    protected virtual void OnRoundEnd(Battle battle)
    {
        DurationCounter.Value++;
        if (DurationCounter.Value >= Duration)
        {
            Entity.Buffs.Remove(this);
            OnDisposalFromField(battle);
        }
    }

}