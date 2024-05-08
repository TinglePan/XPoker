using Godot;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class CrossTierDeBuff: BaseTemporaryBuff
{
    public int Strength;
    
    public CrossTierDeBuff(GameMgr gameMgr, BattleEntity entity, int strength, int duration) : base(
        "Cross tier", "Subjected to a powerful hand. Too powerful that it takes away determination.",
        "res://Sprites/Icons/cross_tier.png", gameMgr, entity, duration)
    {
        Strength = strength;
    }

    public override void OnRoundEnd(Battle battle)
    {
        Entity.Morale.Value = Mathf.Clamp(Entity.Morale.Value - Strength, 0, Entity.MaxMorale.Value);
        base.OnRoundEnd(battle);
    }

}