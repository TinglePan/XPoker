using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class VulnerableDeBuff: BaseTemporaryBuff
{
    public VulnerableDeBuff(int duration, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard) : base(
        "Vulnerable", "Receive 50% more damage", "res://Sprites/BuffIcons/vulnerable.png",
        duration, entity, inflictedBy, inflictedByCard)
    {
        Battle.BeforeApplyAttack += OnBeforeApplyAttack;
    }
    
    private void OnBeforeApplyAttack(Battle battle, Attack attack)
    {
        if (attack.Target == Entity)
        {
            attack.ExtraMultipliers.Add((Configuration.VulnerableMultiplier, Name));
        }
    }
}