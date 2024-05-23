using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class CrossTierDeBuff: BaseTemporaryBuff
{
    public int Power;
    
    public CrossTierDeBuff(int power, int duration, BattleEntity inflictedBy, BaseCard inflictedByCard) : base(
        "Cross tier", "Subjected to a powerful hand. Too powerful that it takes away determination.",
        "res://Sprites/Icons/cross_tier.png", duration, inflictedBy, inflictedByCard)
    {
        Power = power;
    }

    protected override void OnRoundEnd(Battle battle)
    {
        Entity.HitPoint.Value = Mathf.Clamp(Entity.HitPoint.Value - Power, 0, Entity.MaxHitPoint.Value);
        base.OnRoundEnd(battle);
    }

}