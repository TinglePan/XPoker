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
    public Dictionary<Enums.EngageRole, List<BaseSkillEffect>> Contents;
    
    public BaseSkillCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, 
        Dictionary<Enums.EngageRole, List<BaseSkillEffect>> contents, BattleEntity ownerEntity) : 
        base(name, description, iconPath, suit, rank, ownerEntity)
    {
        Contents = contents;
    }

    public bool CanTrigger(Enums.EngageRole role, CompletedHand hand)
    {
        foreach (var content in Contents[role])
        {
            if (content.CanTrigger(hand)) return true;
        }
        return false;
    }
}