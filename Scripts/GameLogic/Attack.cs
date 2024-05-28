using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public class Attack
{
    public BattleEntity Source;
    public BattleEntity Target;
    public CompletedHand SourceHand;
    public CompletedHand TargetHand;
    public int Power;
    public List<BaseSkillCard> SkillCards;
    public List<(int, string)> ExtraDamages;
    public List<(float, string)> ExtraMultipliers;
    
    protected GameMgr GameMgr;
    protected Battle Battle;
    
    public Attack(GameMgr gameMgr, Battle battle, BattleEntity source, BattleEntity target, CompletedHand sourceHand, CompletedHand targetHand)
    {
        GameMgr = gameMgr;
        Battle = battle;
        Source = source;
        Target = target;
        SourceHand = sourceHand;
        TargetHand = targetHand;
        Power = Source.HandPowers[SourceHand.Tier] + source.BaseHandPower;
        ExtraDamages = new List<(int, string)>();
        ExtraMultipliers = new List<(float, string)>();
    }

    public int Damage()
    {
        var damage = Power;
        foreach (var (damageValue, _) in ExtraDamages)
        {
            damage += damageValue;
        }

        float multiplyDamage = damage;
        foreach (var (multiplier, _) in ExtraMultipliers)
        {
            multiplyDamage *= multiplier;
        }
        return Mathf.RoundToInt(multiplyDamage);
    }
    
    public void Apply()
    {
        Target.HitPoint.Value = Mathf.Clamp(Target.HitPoint.Value - Damage(), 0, Target.MaxHitPoint.Value);
    }
}