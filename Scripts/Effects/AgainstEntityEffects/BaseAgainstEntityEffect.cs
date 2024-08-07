using System.Threading.Tasks;

namespace XCardGame;

public class BaseAgainstEntityEffect: BaseEffect
{
    public Engage Engage => Battle.RoundEngage;
    public BattleEntity Src;
    public BattleEntity Dst;
    
    public BaseAgainstEntityEffect(string name, string descriptionTemplate, BaseCard originateCard, BattleEntity src, BattleEntity dst) : 
        base(name, descriptionTemplate, originateCard)
    {
        Src = src;
        Dst = dst;
    }

    public virtual Task Apply()
    {
        return Task.CompletedTask;
    }
    
    
}