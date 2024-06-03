using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class RiposteBuff: BaseTemporaryBuff
{
    public Enums.HandTier HandTier;
    public int Power;
    
    public RiposteBuff(Enums.HandTier handTier, int power, int duration, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard) : base(
        "Riposte", null, "res://Sprites/BuffIcons/vulnerable.png",
        duration, entity, inflictedBy, inflictedByCard)
    {
        Description = $"Negate {handTier} attacks and deal {power} damage to the attacker.";
        HandTier = handTier;
        Power = power;
        Battle.BeforeApplyAttack += OnBeforeApplyAttack;
    }

    protected void OnBeforeApplyAttack(Battle battle, Attack attack)
    {
        if (attack.Target == Entity && attack.SourceHand.Tier == HandTier)
        {
            attack.IsNegated = true;
            attack.Source.ChangeHp(-Power);
        }
    }
}