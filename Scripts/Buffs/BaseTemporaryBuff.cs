using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class BaseTemporaryBuff: BaseBuff, ITemporaryBuff
{
    public int Duration { get; private set; }
    public ObservableProperty<int> Counter { get; private init; }
    public BaseTemporaryBuff(string name, string description, string iconPath, GameMgr gameMgr, BattleEntity entity,
        int duration) : base(name, description, iconPath, gameMgr, entity)
    {
        Duration = duration;
        Counter = new ObservableProperty<int>(nameof(Counter), this, 0);
    }
    
    public override void OnRoundEnd(Battle battle)
    {
        Counter.Value++;
        if (Counter.Value >= Duration)
        {
            Entity.Buffs.Remove(this);
            OnExpired(battle);
        }
    }

}