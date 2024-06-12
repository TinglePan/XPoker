using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class BaseSkillCard: MarkerCard
{
    
    public Enums.HandTier TriggerHandTier;
    public Dictionary<Enums.EngageRole, List<BaseSkillEffect>> Contents;
    
    public bool IsExpanding;
    
    public BaseSkillCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, 
        Enums.HandTier triggerHandTier, Dictionary<Enums.EngageRole, List<BaseSkillEffect>> contents, BattleEntity ownerEntity) : 
        base(name, description, iconPath, suit, rank, ownerEntity)
    {
        TriggerHandTier = triggerHandTier;
        Contents = contents;
        IsExpanding = false;
    }
}