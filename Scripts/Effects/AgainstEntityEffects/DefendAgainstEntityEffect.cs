using System.Threading.Tasks;
using XCardGame.Common;

namespace XCardGame;

public class DefendAgainstEntityEffect: BaseAgainstEntityEffect
{
    public int BaseValue;
    public bool AddHandPower;
    
    public DefendAgainstEntityEffect(BaseCard originateCard, BattleEntity src, BattleEntity dst, int baseValue, bool addHandPower = true) : 
        base(Utils._("Defend"), Utils._($"Gain {baseValue} defence"), originateCard, src, dst)
    {
        BaseValue = baseValue;
        AddHandPower = addHandPower;
    }
    
    public override Task Apply()
    {
        var modifiedDef = BaseValue;
        int defMod = Src.GetDefenceModifier();
        modifiedDef += defMod;
        int handPower = 0;
        if (AddHandPower)
        {
            handPower = Src.HandPowers[Battle.RoundHands[Src].Tier];
            modifiedDef += handPower;
        }
        var separatedMultipliers = Utils.AddUpSeparatedMultipliers(Src.GetAttackMultipliers());
        var res = (int)(modifiedDef * separatedMultipliers.X * separatedMultipliers.Y);

        if (AddHandPower)
        {
            Battle.GameMgr.BattleLog.Log(Utils._($"{Src} defends. Base:{BaseValue}, DefModifier:{defMod}, HandPower:{handPower}, FinalValue:{res}"));
        }
        else
        {
            Battle.GameMgr.BattleLog.Log(Utils._($"{Src} defends. Base:{BaseValue}, DefModifier:{defMod}, FinalValue:{res}"));
        }
        
        Src.ChangeGuard(res);
        Battle.GameMgr.BattleLog.Log(Utils._($"Gained {res} guard."));
        return Task.CompletedTask;
    }
}