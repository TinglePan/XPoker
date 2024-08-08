using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

public class HealEffect: BaseAgainstEntityEffect
{
    public int BaseValue;
    public HealEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int baseValue) : 
        base(Utils._("Heal"), Utils._($"Heal {baseValue}"), originateCard, src, dst)
    {
        BaseValue = baseValue;
    }
    
    public override Task Apply()
    {
        Src.ChangeHp(BaseValue);
        Battle.GameMgr.BattleLog.Log(Utils._($"{Src} heals for {BaseValue}"));
        return Task.CompletedTask;
    }
}