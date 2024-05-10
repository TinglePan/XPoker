using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public class AttackObj
{
    public BattleEntity Source;
    public BattleEntity Target;
    public CompletedHand SourceHand;
    public CompletedHand SourceHandWithoutFaceDownCards;
    public CompletedHand TargetHand;
    public CompletedHand TargetHandWithoutFaceDownCards;
    public int BaseDamage;
    public Dictionary<string, int> ExtraDamages;

    public int Damage => BaseDamage + ExtraDamages.Values.Sum();
    
    protected GameMgr GameMgr;
    
    public AttackObj(GameMgr gameMgr, BattleEntity source, BattleEntity target, CompletedHand sourceHand,
        CompletedHand sourceHandWithoutFaceDownCards, CompletedHand targetHand, CompletedHand targetHandWithoutFaceDownCards)
    {
        GameMgr = gameMgr;
        Source = source;
        Target = target;
        SourceHand = sourceHand;
        SourceHandWithoutFaceDownCards = sourceHandWithoutFaceDownCards;
        TargetHand = targetHand;
        TargetHandWithoutFaceDownCards = targetHandWithoutFaceDownCards;
        BaseDamage = Source.DamageTable[SourceHand.Tier];
        ExtraDamages = new Dictionary<string, int>();
    }

    public bool IsWinByOuts()
    {
        return SourceHand.CompareTo(TargetHand) > 0 && SourceHandWithoutFaceDownCards.CompareTo(TargetHandWithoutFaceDownCards) < 0;
    }
    
    public void Apply()
    {
        Target.TakeDamage(Damage, Source);
        if (SourceHand.Tier - TargetHand.Tier >= Target.CrossTierThreshold)
        {
            Source.InflictBuffOn(new CrossTierDeBuff(GameMgr, Target, SourceHand.Tier - TargetHand.Tier, 1), Target);
        }
    }
}