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
    public int BaseDamage;
    public List<BaseSkillCard> SkillCards;
    public Dictionary<string, int> ExtraDamages;
    public Dictionary<string, float> ExtraMultipliers;

    public int Damage => BaseDamage + ExtraDamages.Values.Sum();
    
    protected GameMgr GameMgr;
    
    public Attack(GameMgr gameMgr, BattleEntity source, BattleEntity target, CompletedHand sourceHand, CompletedHand targetHand)
    {
        GameMgr = gameMgr;
        Source = source;
        Target = target;
        SourceHand = sourceHand;
        TargetHand = targetHand;
        BaseDamage = Source.HandPowers[SourceHand.Tier];
        ExtraDamages = new Dictionary<string, int>();
        ExtraMultipliers = new Dictionary<string, float>();
    }
    
    public void Apply()
    {
        Target.HitPoint.Value = Mathf.Clamp(Target.HitPoint.Value - Damage, 0, Target.MaxHitPoint.Value);
    }
}