using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class CrossTierDeBuff: BaseTemporaryBuff
{
    public int Power;
    
    public CrossTierDeBuff(int power, int duration, BattleEntity entity, BattleEntity inflictedBy, BaseCard inflictedByCard) : 
        base("Cross tier", "Subjected to a powerful hand. Too powerful that it takes away determination.",
        "res://Sprites/Icons/cross_tier.png", duration, entity, inflictedBy, inflictedByCard)
    {
        Power = power;
    }

    protected override void OnRoundEnd(Battle battle)
    {
        Entity.Hp.Value = Mathf.Clamp(Entity.Hp.Value - Power, 0, Entity.MaxHp.Value);
        base.OnRoundEnd(battle);
    }

}