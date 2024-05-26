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
    public CompletedHand SourceHandWithoutFaceDownCards;
    public CompletedHand TargetHand;
    public CompletedHand TargetHandWithoutFaceDownCards;
    public int BaseDamage;
    public List<BaseSkillCard> SkillCards;
    public Dictionary<string, int> ExtraDamages;
    public Dictionary<string, float> ExtraMultipliers;

    public int Damage => BaseDamage + ExtraDamages.Values.Sum();
    
    protected GameMgr GameMgr;
    
    public Attack(GameMgr gameMgr, BattleEntity source, BattleEntity target, CompletedHand sourceHand,
        CompletedHand sourceHandWithoutFaceDownCards, CompletedHand targetHand, CompletedHand targetHandWithoutFaceDownCards)
    {
        GameMgr = gameMgr;
        Source = source;
        Target = target;
        SourceHand = sourceHand;
        SourceHandWithoutFaceDownCards = sourceHandWithoutFaceDownCards;
        TargetHand = targetHand;
        TargetHandWithoutFaceDownCards = targetHandWithoutFaceDownCards;
        BaseDamage = Source.HandPowers[SourceHand.Tier];
        ExtraDamages = new Dictionary<string, int>();
        ExtraMultipliers = new Dictionary<string, float>();
    }

    public bool IsWinByOuts()
    {
        return SourceHand.CompareTo(TargetHand) > 0 && SourceHandWithoutFaceDownCards.CompareTo(TargetHandWithoutFaceDownCards) < 0;
    }
    
    public void Apply()
    {
        Target.HitPoint.Value = Mathf.Clamp(Target.HitPoint.Value - Damage, 0, Target.MaxHitPoint.Value);
    }
}