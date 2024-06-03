using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public class Attack
{
    public BattleEntity Source;
    public BattleEntity Target;
    public CompletedHand SourceHand;
    public CompletedHand TargetHand;
    public int Power;
    public List<BaseSkillCard> RelevantSkillCards;
    public List<(int, string)> ExtraDamages;
    public List<(float, string)> ExtraMultipliers;
    public bool IsNegated;
    
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
        RelevantSkillCards = new List<BaseSkillCard>();
        IsNegated = false;
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
        if (!IsNegated)
        {
            var actualDamage = Mathf.Clamp(Damage(), 0, Target.Hp.Value);
            Target.Hp.Value -= actualDamage;
            foreach (var skillCard in RelevantSkillCards)
            {
                skillCard.AfterDamageDealt(this, actualDamage);
            }
        }
    }
}